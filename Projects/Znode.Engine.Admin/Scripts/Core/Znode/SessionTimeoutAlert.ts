class SessionTimeoutAlert extends ZnodeBase {
    _idleTime: number;

    constructor() {
        super();
        SessionTimeoutAlert.prototype._idleTime = 0;
    }

    DisplayAlertMessage(): void {
        setInterval(SessionTimeoutAlert.prototype.TimeIncrement, 60000); // 60000 = 1 minute

        //Zero the idle timer on mouse movement.
        $('body').mousemove(function (e) {
            SessionTimeoutAlert.prototype._idleTime = 0;
        });

        $('body').keypress(function (e) {
            SessionTimeoutAlert.prototype._idleTime = 0;
        });

        $('body').click(function () {
            SessionTimeoutAlert.prototype._idleTime = 0;
        });
    }

    TimeIncrement(): void {
        SessionTimeoutAlert.prototype._idleTime = SessionTimeoutAlert.prototype._idleTime + 1;
        var sessionTimeoutWarning: number = $("#SessionWarningTime").val();
        if (SessionTimeoutAlert.prototype._idleTime > sessionTimeoutWarning) {
            //alert("Your session will expire in another 2 mins! Please Save the data before the session expires");
        }
    }
}
