# DynamicIPChecker
Simple service that tracks your machine public IP address and in case it's changed - sends new one to your email.

# Setup
Find *App.config* file and open it.
In appSettings section you can customize:
- Sender's email (*emailFrom*): this email account is used to send message with new IP;
- Reciver's email (*emailTo*): email where you get notifications to;
- Subject of message (*emailSubject*): subject of message - I use it for email filtering;
- Email template (*emailBodyTemplate*): template of the message {0} - PC name, {1} - IP address;
- Checking interval in ms (*checkInterval*): IP will be checked every X milliseconds;
In mailSettings section you can enter you email credentials.

# ToDo
- add more "IP checker"-services in case main one will be unavailable.
- add UI for settings
- add multiple emailTo support 
