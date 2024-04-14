
public abstract class AbstractWave
{
    public float spawnTime;
    public bool finished = false;

    public AbstractWave(float spawnTime)
    {
        this.spawnTime = spawnTime;
    }

    public abstract void DoSpawn();
}