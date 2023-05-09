using UnityEngine;

public class TowerButton : MonoBehaviour {
    [SerializeField]
    private Tower towerObject;
    [SerializeField]
    private Sprite dragSprite;
    [SerializeField]
    private int towerPrice;

    public Tower TowerObject => towerObject;

    public Sprite DragSprite => dragSprite;

    public int TowerPrice => towerPrice;
}
