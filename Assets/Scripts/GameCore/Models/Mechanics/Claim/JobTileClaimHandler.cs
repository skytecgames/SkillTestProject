using UnityEngine;
using System.Collections;

//Класс следит, чтобы при остановке или завершении задачи снять клейм с тайла
public class JobTileClaimHandler
{
    private JobNew job;
    private ClaimType claimType;
    private Tile tile;

    public JobTileClaimHandler(JobNew job, Tile tile, ClaimType claimType)
    {
        if (job == null) {
            Debug.LogError("JobTileClaimHandler error: job is null");
            return;
        }

        this.job = job;
        this.claimType = claimType;
        this.tile = tile;

        job.cbJobComplete += OnJobStop;
        job.cbJobStop += OnJobStop;
    }

    private void OnJobStop(JobNew job)
    {
        if(job == null || tile == null) {
            Debug.LogError("JobTileClaimHandler job stop more than one times");
            return;
        }

        job.cbJobComplete -= OnJobStop;
        job.cbJobStop -= OnJobStop;

        TileClaimManager.UnClaim(tile, claimType);

        job = null;
        tile = null;
    }
}
