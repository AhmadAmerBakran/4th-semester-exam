import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';

class CarControlWidget extends StatelessWidget {
  final TextEditingController _topicController = TextEditingController();
  final TextEditingController _commandController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.all(16.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          TextField(
            controller: _topicController,
            decoration: InputDecoration(labelText: 'Topic'),
          ),
          TextField(
            controller: _commandController,
            decoration: InputDecoration(labelText: 'Command'),
          ),
          SizedBox(height: 20),
          ElevatedButton(
            onPressed: () {
              final topic = _topicController.text;
              final command = _commandController.text;
              if (topic.isNotEmpty && command.isNotEmpty) {
                Provider.of<CarControlProvider>(context, listen: false)
                    .sendCommand(topic, command);
              }
            },
            child: Text('Send Command'),
          ),
        ],
      ),
    );
  }
}