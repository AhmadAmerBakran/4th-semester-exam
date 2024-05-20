import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
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
    final carControlProvider = Provider.of<CarControlProvider>(context, listen: false);
    final webSocketService = carControlProvider.webSocketService;

    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Container(
          width: MediaQuery.of(context).size.width > 600 ? 400 : MediaQuery.of(context).size.width * 0.8,
          child: TextField(
            controller: nicknameController,
            style: GoogleFonts.rowdies(),
            decoration: InputDecoration(
              labelText: 'Nickname',
              labelStyle: GoogleFonts.rowdies(),
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
              Provider.of<UserProvider>(context, listen: false).setUser(user);
              carControlProvider.signIn(user);


              webSocketService.messageStream.listen((message) {
                if (_isJson(message)) {
                  final decodedMessage = jsonDecode(message);
                  if (decodedMessage['eventType'] == 'ServerClientSignIn' &&
                      decodedMessage['Message'] == 'You have connected as $nickname') {
                    Navigator.pushReplacementNamed(context, '/carControl');
                  }
                } else {
                  print("Non-JSON message received: $message");
                }
              });
            }
          },
          child: Text('Start', style: GoogleFonts.rowdies()),
        ).animate().slide(duration: 800.ms, begin: Offset(1, 0), end: Offset(0, 0)).then().shimmer(),
      ],
    );
  }


  bool _isJson(String str) {
    try {
      jsonDecode(str);
      return true;
    } catch (e) {
      return false;
    }
  }
}
