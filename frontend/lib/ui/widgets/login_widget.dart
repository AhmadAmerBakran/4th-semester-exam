import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';
import 'package:flutter_animate/flutter_animate.dart';

class LoginWidget extends StatelessWidget {
  final TextEditingController _nicknameController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.all(16.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Image.asset(
            'assets/car.png', // Make sure you have this asset in your project
            height: 100,
          ).animate().scale(duration: 500.ms).then().shake(hz: 2, curve: Curves.easeInOut),
          SizedBox(height: 20),
          TextField(
            controller: _nicknameController,
            decoration: InputDecoration(
              labelText: 'Nickname',
              border: OutlineInputBorder(),
            ),
          ).animate().fadeIn(duration: 800.ms).then(delay: 500.ms).shimmer(),
          SizedBox(height: 20),
          ElevatedButton(
            onPressed: () {
              final nickname = _nicknameController.text;
              if (nickname.isNotEmpty) {
                Provider.of<CarControlProvider>(context, listen: false).signIn(nickname);
                Navigator.pushReplacementNamed(context, '/carControl');
              }
            },
            child: Text('Login'),
          ).animate().slide(duration: 800.ms, begin: Offset(1, 0), end: Offset(0, 0)).then().shimmer(),
        ],
      ),
    );
  }
}