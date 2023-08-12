using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Ent : Monster
{
    public override int Money => 0;
    public override bool HasSoul => false;
    public override bool Movable => !treeForm;

    public override bool Threatening => !treeForm;

    public int cooldown;
    public int currentCooldown;

    public bool treeForm = true;
    public SpriteRenderer treeSprite;
    public GameObject entModel;
    public GameObject activeSprite;
    public GameObject inactiveSprite; 

    public override void Awake() {
        base.Awake();
        currentCooldown = cooldown;

        ChangeForm(tree: true);
        new ValueTracker<bool>(() => treeForm, ChangeForm);

        new ValueTracker<int>(() => currentCooldown, v => {
            currentCooldown = v;
            UpdateSprite();
        });
    }

    private void UpdateSprite() {
        activeSprite.SetActive(currentCooldown <= 1);
        inactiveSprite.SetActive(currentCooldown > 1);
        //actives
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (treeForm) {
            return false;
        }
        return await base.TryAttack(delta);
    }

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        if (alive) {
            attack.afterAttack.Add(async () => ChangeForm(tree: false));
        }
    }

    private void ChangeForm(bool tree) {
        treeForm = tree;
        treeSprite.enabled = treeForm;
        entModel.SetActive(!tree);
    }

    protected override async Task MakeMove() {
        if (treeForm) {
            return;
        }

        --currentCooldown;
        UpdateSprite();
        if (currentCooldown > 0) {
            return;
        }

        var playerDelta = Player.instance.figure.Location.position - figure.Location.position;
        playerDelta = Helpers.StepAtDirection(playerDelta);
        GetComponent<SpriteDirection>().SetDirection(playerDelta);
        if (!await SmartWalk(playerDelta)) {
            if (!await TryAttack(playerDelta)) {
                await SmartFakeMove(playerDelta);
            }
        }
        currentCooldown = cooldown;
        UpdateSprite();
    }
}
