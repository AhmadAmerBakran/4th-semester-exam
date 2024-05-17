import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:carousel_slider/carousel_slider.dart';
import '../../providers/car_control_provider.dart';
import 'package:flutter_animate/flutter_animate.dart';

class LoginWidget extends StatefulWidget {
  @override
  _LoginWidgetState createState() => _LoginWidgetState();
}

class _LoginWidgetState extends State<LoginWidget> with TickerProviderStateMixin {
  final TextEditingController _nicknameController = TextEditingController();
  late AnimationController _typingController;
  late Animation<int> _typingAnimation;
  late AnimationController _cursorController;
  late Animation<double> _cursorAnimation;
  late ScrollController _scrollController;
  final String _welcomeText = """
Welcome to our Car Control project!

With this platform, you can easily control our car which is connected to an ESP microcontroller and receive real-time video streaming from it. No authentication or authorization is required—just enter your nickname and start exploring.

Features:
• Control the IoT Car: Seamlessly navigate the car from your device.
• Real-Time Video Stream: Get instant video feedback from the car’s camera.

Developed by: Mahmoud Eybo and Ahmad Amer Bakran
""";

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _typingController = AnimationController(
      duration: const Duration(seconds: 10),
      vsync: this,
    )..addListener(() {
      if (_scrollController.hasClients) {
        _scrollController.animateTo(
          _scrollController.position.maxScrollExtent,
          duration: Duration(milliseconds: 100),
          curve: Curves.easeInOut,
        );
      }
    });

    _typingAnimation = StepTween(begin: 0, end: _welcomeText.length).animate(_typingController);
    _typingController.forward();

    _cursorController = AnimationController(
      duration: const Duration(milliseconds: 500),
      vsync: this,
    )..repeat(reverse: true);

    _cursorAnimation = Tween<double>(begin: 0.0, end: 1.0).animate(_cursorController);
  }

  @override
  void dispose() {
    _typingController.dispose();
    _cursorController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    double screenWidth = MediaQuery.of(context).size.width;
    double screenHeight = MediaQuery.of(context).size.height;
    double textContainerWidth = screenWidth > 600 ? 400 : screenWidth * 0.8;
    double textContainerHeight = screenWidth > 600 ? 200 : screenHeight * 0.25;

    return LayoutBuilder(
      builder: (context, constraints) {
        return Stack(
          children: [
            Positioned.fill(
              child: CarouselSlider(
                options: CarouselOptions(
                  height: double.infinity,
                  viewportFraction: 1.0,
                  autoPlay: true,
                  autoPlayInterval: Duration(seconds: 5),
                  autoPlayAnimationDuration: Duration(seconds: 1),
                  autoPlayCurve: Curves.easeInOut,
                  aspectRatio: MediaQuery.of(context).size.aspectRatio,
                  enlargeCenterPage: false,
                  scrollDirection: Axis.horizontal,
                ),
                items: [
                  'assets/images/car1.jpg',
                  'assets/images/car2.jpg',
                  'assets/images/car3.jpg'
                ].map((imagePath) {
                  return Container(
                    decoration: BoxDecoration(
                      image: DecorationImage(
                        image: AssetImage(imagePath),
                        fit: BoxFit.cover,
                        colorFilter: ColorFilter.mode(Colors.black.withOpacity(0.6), BlendMode.dstATop),
                      ),
                    ),
                  );
                }).toList(),
              ),
            ),
            Positioned(
              top: 16.0,
              left: screenWidth <= 600 ? (screenWidth - textContainerWidth) / 2 : 16.0,
              child: Container(
                width: textContainerWidth,
                height: textContainerHeight,
                padding: EdgeInsets.all(12.0),
                decoration: BoxDecoration(
                  color: Colors.black.withOpacity(0.7),
                  borderRadius: BorderRadius.circular(10.0),
                ),
                child: SingleChildScrollView(
                  controller: _scrollController,
                  child: AnimatedBuilder(
                    animation: Listenable.merge([_typingAnimation, _cursorAnimation]),
                    builder: (context, child) {
                      String text = _welcomeText.substring(0, _typingAnimation.value);
                      return Text.rich(
                        TextSpan(
                          text: text,
                          style: TextStyle(
                            fontSize: 16,
                            color: Colors.white,
                          ),
                          children: [
                            TextSpan(
                              text: _cursorAnimation.value > 0.5 ? '|' : '',
                              style: TextStyle(
                                fontSize: 16,
                                color: Colors.greenAccent,
                              ),
                            ),
                          ],
                        ),
                      );
                    },
                  ),
                ),
              ),
            ),
            Positioned(
              top: screenHeight > 600 ? screenHeight * 0.35 : screenHeight * 0.3,
              left: 16.0,
              right: 16.0,
              child: Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Container(
                      width: constraints.maxWidth > 600 ? 400 : constraints.maxWidth * 0.8,
                      child: TextField(
                        controller: _nicknameController,
                        decoration: InputDecoration(
                          labelText: 'Nickname',
                          border: OutlineInputBorder(),
                          fillColor: Colors.white.withOpacity(0.8),
                          filled: true,
                        ),
                      ).animate().fadeIn(duration: 800.ms).then(delay: 500.ms).shimmer(),
                    ),
                    SizedBox(height: 20),
                    ElevatedButton(
                      onPressed: () {
                        final nickname = _nicknameController.text;
                        if (nickname.isNotEmpty) {
                          Provider.of<CarControlProvider>(context, listen: false).signIn(nickname);
                          Navigator.pushReplacementNamed(context, '/carControl');
                        }
                      },
                      child: Text('Start'),
                    ).animate().slide(duration: 800.ms, begin: Offset(1, 0), end: Offset(0, 0)).then().shimmer(),
                  ],
                ),
              ),
            ),
          ],
        );
      },
    );
  }
}
