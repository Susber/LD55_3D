using UnityEngine;

namespace Components.Levels
{
    public class SpawnRune : AbstractWave
    {
        private RuneController.RuneType runeType;
        private RuneController.SummonEffect runeEffect;
        
        public SpawnRune(float spawnTime, RuneController.RuneType runeType, RuneController.SummonEffect runeEffect) : base(spawnTime)
        {
            this.runeType = runeType;
            this.runeEffect = runeEffect;
        }

        public override void DoSpawn()
        {
            var position = ArenaController.Instance.RandomRunePos();

            var prefab = ArenaController.Instance.runePrefab;
            var rune = Object.Instantiate(prefab, ArenaController.Instance.runeContainer);
            rune.transform.localPosition = position;
            var runeController = rune.GetComponent<RuneController>();
            var edges = runeType.MakeEdges();
            runeController.MakeRuneFromEdges(edges, position);
            runeController.needsToStartAtEnd = !edges.closedLoop;
            runeController.summonEffect = runeEffect;
            finished = true;
        }
    }
}
