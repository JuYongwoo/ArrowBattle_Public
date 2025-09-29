using UnityEngine;
public class CharacterStatManager //Player�� Enemy ��� �� Ŭ������ ��ü�� ��������
{
    public struct CharacterStat
    {
        public CharacterStat(CharacterStatDataSO playerData)
        {
            this.MaxHP = playerData.MaxHP;
            this.CurrentHP = playerData.CurrentHP;
            this.CurrentMoveSpeed = playerData.CurrentMoveSpeed;
            this.HitSound = playerData.HitSound;
        }

        public float MaxHP;
        public float CurrentHP;
        public float CurrentMoveSpeed;
        public AudioClip HitSound;

    }

    public CharacterStat Current; //��� ��ġ�� ���ؾ��ϹǷ� readonly ��� X




    //TODO JYW �÷��̾�� ������ ���� ��ų�� ��ϵ� ���⼭ ���簡��

    public CharacterStatManager(CharacterTypeEnumByTag type)
    {
        var playerData = ManagerObject.instance.resourceManager.playerDatas[type].Result;

        Current = new CharacterStat(playerData);

    }

    public void deltaHP(float delta)
    {
        Current.CurrentHP += delta;
    }

}
