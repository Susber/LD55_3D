using UnityEngine;

namespace Components.Levels
{
    public class SpawnRune : AbstractWave
    {
        private float runeRadius;
        
        public SpawnRune(float spawnTime, float runeRadius) : base(spawnTime)
        {
            this.runeRadius = runeRadius;
        }

        public override void DoSpawn()
        {
            var prefab = ArenaController.Instance.runePrefab;
            var rune = Object.Instantiate(prefab, ArenaController.Instance.runeContainer);
            rune.transform.localPosition = Vector3.zero;
            rune.GetComponent<RuneController>().MakePentagram(5, runeRadius, 1f);
            finished = true;
        }
    }
}