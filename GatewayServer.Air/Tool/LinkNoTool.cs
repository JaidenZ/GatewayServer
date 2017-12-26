namespace GatewayServer.Air.Tool
{

    public static class LinkNoTool
    {
        private static int _linkno = 0;

        public static int GetLinkNo()
        {
            if (_linkno == int.MaxValue)
                _linkno = 0;
            _linkno++;
            return _linkno;
        }


    }
}
