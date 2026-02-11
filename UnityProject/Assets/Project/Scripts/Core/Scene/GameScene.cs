using System;

public abstract class GameScene
{
    public abstract void Init(Action action = null);
    public abstract void DeInit(Action action = null);
    public abstract void DataInitialize(Action action = null);
}
