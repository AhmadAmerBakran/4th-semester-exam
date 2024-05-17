import 'dart:ui' as ui;
import 'package:flutter/material.dart';

class StreamWidget extends StatelessWidget {
  final ui.Image currentImage;

  StreamWidget({required this.currentImage});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        border: Border.all(color: Colors.blueAccent),
        borderRadius: BorderRadius.circular(10),
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(10),
        child: CustomPaint(
          painter: FramePainter(currentImage),
        ),
      ),
    );
  }
}

class FramePainter extends CustomPainter {
  final ui.Image image;

  FramePainter(this.image);

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint();
    canvas.drawImage(image, Offset.zero, paint);
  }

  @override
  bool shouldRepaint(FramePainter oldDelegate) {
    return oldDelegate.image != image;
  }
}