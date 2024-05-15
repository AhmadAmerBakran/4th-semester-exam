class CarControlCommand {
  final String eventType;
  final String topic;
  final String command;

  CarControlCommand({required this.eventType, required this.topic, required this.command});

  factory CarControlCommand.fromJson(Map<String, dynamic> json) {
    return CarControlCommand(
      eventType: json['eventType'],
      topic: json['Topic'],
      command: json['Command'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'eventType': eventType,
      'Topic': topic,
      'Command': command,
    };
  }
}

class SignInEvent {
  final String eventType;
  final String nickName;

  SignInEvent({required this.eventType, required this.nickName});

  factory SignInEvent.fromJson(Map<String, dynamic> json) {
    return SignInEvent(
      eventType: json['eventType'],
      nickName: json['NickName'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'eventType': eventType,
      'NickName': nickName,
    };
  }
}

class SignOutEvent {
  final String eventType;

  SignOutEvent({required this.eventType});

  factory SignOutEvent.fromJson(Map<String, dynamic> json) {
    return SignOutEvent(
      eventType: json['eventType'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'eventType': eventType,
    };
  }
}

class ReceiveNotificationsEvent {
  final String eventType;

  ReceiveNotificationsEvent({required this.eventType});

  factory ReceiveNotificationsEvent.fromJson(Map<String, dynamic> json) {
    return ReceiveNotificationsEvent(
      eventType: json['eventType'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'eventType': eventType,
    };
  }
}

class GetCarLogEvent {
  final String eventType;

  GetCarLogEvent({required this.eventType});

  factory GetCarLogEvent.fromJson(Map<String, dynamic> json) {
    return GetCarLogEvent(
      eventType: json['eventType'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'eventType': eventType,
    };
  }
}