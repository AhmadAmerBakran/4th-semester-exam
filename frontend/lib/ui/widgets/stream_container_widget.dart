import 'dart:io';
import 'dart:typed_data';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter_ffmpeg/flutter_ffmpeg.dart';
import 'package:image_gallery_saver/image_gallery_saver.dart';
import 'package:path_provider/path_provider.dart';
import 'package:path/path.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:universal_html/html.dart' as html;
import 'dart:ui' as ui;
import 'package:frontend/ui/widgets/stream_widget.dart';

class StreamContainer extends StatefulWidget {
  final bool isStreaming;
  final ui.Image? currentImage;

  StreamContainer({required this.isStreaming, required this.currentImage});

  @override
  _StreamContainerState createState() => _StreamContainerState();
}

class _StreamContainerState extends State<StreamContainer> {
  final GlobalKey repaintBoundaryKey = GlobalKey();
  bool _isRecording = false;
  final FlutterFFmpeg _flutterFFmpeg = FlutterFFmpeg();
  String? _videoFilePath;
  late Directory _appDir;

  @override
  void initState() {
    super.initState();
    _initialize();
    _requestPermissions();
  }

  Future<void> _initialize() async {
    if (!kIsWeb) {
      _appDir = await getApplicationDocumentsDirectory();
    }
  }

  Future<void> _requestPermissions() async {
    await [
      Permission.storage,
      Permission.microphone,
      Permission.camera,
      Permission.manageExternalStorage,
    ].request();
  }

  Future<void> _takeScreenshot() async {
    try {
      RenderRepaintBoundary boundary = repaintBoundaryKey.currentContext?.findRenderObject() as RenderRepaintBoundary;
      ui.Image image = await boundary.toImage(pixelRatio: 2.0);
      ByteData? byteData = await image.toByteData(format: ui.ImageByteFormat.png);
      if (byteData != null) {
        Uint8List pngBytes = byteData.buffer.asUint8List();

        if (kIsWeb) {
          final blob = html.Blob([pngBytes], 'image/png');
          final url = html.Url.createObjectUrlFromBlob(blob);
          final anchor = html.AnchorElement(href: url)
            ..setAttribute('download', 'screenshot.png')
            ..click();
          html.Url.revokeObjectUrl(url);
        } else {
          final result = await ImageGallerySaver.saveImage(pngBytes);
          print("Screenshot saved: $result");
        }
      }
    } catch (e) {
      print("Error taking screenshot: $e");
    }
  }

  Future<void> _startRecording() async {
    try {
      if (!kIsWeb) {
        _videoFilePath = join(_appDir.path, "recorded_video.mp4");
        // Add FFmpeg command to start recording
        final arguments = [
          "-f", "x11grab",
          "-r", "30",
          "-s", "${widget.currentImage!.width}x${widget.currentImage!.height}",
          "-i", ":0.0",
          "-vcodec", "libx264",
          "-preset", "ultrafast",
          _videoFilePath!
        ];

        _flutterFFmpeg.executeWithArguments(arguments).then((rc) => print("FFmpeg process exited with rc $rc"));

        setState(() {
          _isRecording = true;
        });
      } else {
        print("Recording is not supported on web.");
      }
    } catch (e) {
      print("Error starting recording: $e");
    }
  }

  Future<void> _stopRecording() async {
    try {
      if (!kIsWeb) {
        _flutterFFmpeg.cancel();
        setState(() {
          _isRecording = false;
        });
        print("Recording saved to $_videoFilePath");

        if (Platform.isWindows || Platform.isMacOS || Platform.isLinux) {
          Process.run('explorer', [dirname(_videoFilePath!)]);
        }
      } else {
        print("Recording is not supported on web.");
      }
    } catch (e) {
      print("Error stopping recording: $e");
    }
  }

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Stack(
        children: [
          Container(
            width: MediaQuery.of(context).size.width * 0.47,
            height: MediaQuery.of(context).size.height * 0.47,
            decoration: BoxDecoration(
              border: Border.all(color: Colors.blueAccent),
              borderRadius: BorderRadius.circular(10),
            ),
            child: RepaintBoundary(
              key: repaintBoundaryKey,
              child: widget.isStreaming
                  ? widget.currentImage != null
                  ? StreamWidget(currentImage: widget.currentImage!)
                  : Center(child: CircularProgressIndicator())
                  : Center(child: Text('Stream not started')),
            ),
          ),
          Positioned(
            left: 16,
            top: 16,
            child: IconButton(
              icon: Icon(Icons.camera_alt),
              onPressed: _takeScreenshot,
              tooltip: 'Take Screenshot',
              color: Colors.blue,
              iconSize: 30.0,
            ),
          ),
          Positioned(
            right: 16,
            top: 16,
            child: IconButton(
              icon: Icon(_isRecording ? Icons.stop : Icons.videocam),
              onPressed: _isRecording ? _stopRecording : _startRecording,
              tooltip: _isRecording ? 'Stop Recording' : 'Start Recording',
              color: Colors.red,
              iconSize: 30.0,
            ),
          ),
        ],
      ),
    );
  }
}
