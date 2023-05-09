using UnityEngine;

public class SoundManager : MonoBehaviour {

    [SerializeField]
    private AudioClip arrowClip;
    [SerializeField]
    private AudioClip deathClip;
    [SerializeField]
    private AudioClip fireballClip;
    [SerializeField]
    private AudioClip gameOverClip;
    [SerializeField]
    private AudioClip hitClip;
    [SerializeField]
    private AudioClip levelClip;
    [SerializeField]
    private AudioClip newGameClip;
    [SerializeField]
    private AudioClip rockClip;
    [SerializeField]
    private AudioClip towerBuiltClip;

    public AudioClip ArrowClip => arrowClip;

    public AudioClip DeathClip => deathClip;

    public AudioClip FireballClip => fireballClip;

    public AudioClip GameOverClip => gameOverClip;

    public AudioClip HitClip => hitClip;

    public AudioClip LevelClip => levelClip;

    public AudioClip NewGameClip => newGameClip;

    public AudioClip RockClip => rockClip;

    public AudioClip TowerBuiltClip => towerBuiltClip;
}
