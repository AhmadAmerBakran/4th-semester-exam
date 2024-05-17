import 'package:flutter/material.dart';
import 'package:frontend/ui/widgets/stream_widget.dart';
import '../../utils/constants.dart';
import 'dart:ui' as ui;

class StreamContainer extends StatelessWidget {
  final bool isStreaming;
  final ui.Image? currentImage;

  StreamContainer({required this.isStreaming, required this.currentImage});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: MediaQuery.of(context).size.width * 0.47,
      height: MediaQuery.of(context).size.height * 0.47,
      child: Center(
        child: Container(
          width: MediaQuery.of(context).size.width > 800 ? kWebWidth : kMobileWidth,
          height: MediaQuery.of(context).size.height * 0.47,
          decoration: BoxDecoration(
            border: Border.all(color: Colors.blueAccent),
            borderRadius: BorderRadius.circular(10),
          ),
          child: isStreaming
              ? currentImage != null
              ? StreamWidget(currentImage: currentImage!)
              : Center(child: CircularProgressIndicator())
              : Center(child: Text('Stream not started')),
        ),
      ),
    );
  }
}