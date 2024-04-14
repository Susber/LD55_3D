using UnityEngine;

namespace Components.Levels
{
    public class SpawnRune : AbstractWave
    {
        private RuneController.RuneType runeType;
        
        public SpawnRune(float spawnTime, RuneController.RuneType runeType) : base(spawnTime)
        {
            this.runeType = runeType;
        }

        public override void DoSpawn()
        {
            var prefab = ArenaController.Instance.runePrefab;
            var rune = Object.Instantiate(prefab, ArenaController.Instance.runeContainer);
            rune.transform.localPosition = Vector3.zero;
            var runeController = rune.GetComponent<RuneController>();
            var edges = runeType.MakeEdges();
            runeController.MakeRuneFromEdges(edges);
            runeController.needsToStartAtEnd = !edges.closedLoop;
            finished = true;
        }
    }
}
