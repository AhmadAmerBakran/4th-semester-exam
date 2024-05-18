import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../models/user.dart';
import '../../providers/car_control_provider.dart';
import 'package:flutter_animate/flutter_animate.dart';

import '../../providers/user_provider.dart';

class LoginForm extends StatelessWidget {
  final TextEditingController nicknameController;
  LoginForm({required this.nicknameController});

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Container(
          width: MediaQuery.of(context).size.width > 600 ? 400 : MediaQuery.of(context).size.width * 0.8,
          child: TextField(
            controller: nicknameController,
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
            final nickname = nicknameController.text;
            if (nickname.isNotEmpty) {
              FocusScope.of(context).unfocus();
              final user = User(nickname: nickname);
              Future.delayed(Duration(milliseconds: 400), () {
                Provider.of<UserProvider>(context, listen: false).setUser(user);
                Provider.of<CarControlProvider>(context, listen: false).signIn(user);
                Navigator.pushReplacementNamed(context, '/carControl');
              });
            }
          },
          child: Text('Start'),
        ).animate().slide(duration: 800.ms, begin: Offset(1, 0), end: Offset(0, 0)).then().shimmer(),
      ],
    );
  }
}
