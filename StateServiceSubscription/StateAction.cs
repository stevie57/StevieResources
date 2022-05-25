using System;

public struct ServiceSubscription
{
    public IStateListener Service;
    public Action SetupCallback;
    public Action TearDownCallback;

    public ServiceSubscription(IStateListener service, Action Setup, Action TearDown)
    {
        Service = service;
        SetupCallback = Setup;
        TearDownCallback = TearDown;
    }
}