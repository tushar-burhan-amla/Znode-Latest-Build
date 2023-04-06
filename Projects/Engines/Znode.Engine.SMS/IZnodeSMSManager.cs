namespace Znode.Engine.SMS
{
    public interface IZnodeSMSManager
    {
        /// <summary>
        /// This Method is used to send SMS based on Portal and Template setting
        /// </summary>
        /// <param name="znodeSmsContext"></param>
        void SendSMS(ZnodeSMSContext znodeSmsContext);
    }
}
