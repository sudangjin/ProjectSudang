public class UpgradeStat
{
    public int AddPlayerDamage { get; private set; }
    public float PaneltyPlayerDamage { get; private set; } = 1f;
    public float MultplePlayerProjectileSpeed { get; private set; }
    public int AddShotSameDir { get; private set; }
    public int AddShotRandomDir { get; private set; }
    public bool AddShotBehindDir { get; private set; }
    public float MultiplePlayerAttackSpeed { get; private set; }
    public float MultipleTurnSpeed { get; private set; }
    public bool NormalizeShotDir { get; private set; }
    public int AddHealPerSecond { get; private set; }
    public int AddHealOnKill { get; private set; }
    public float MultipleHealAmount { get; private set; }
    public int AddHP { get; private set; }
    public float MultipleAppearEnemyTime { get; private set; }
    public int AddEnemyCountByWave { get; private set; }
    public int StartAddEnemyCountByWave { get; private set; }
    public float MultipleEnemyCount { get; private set; }


    public float MultipleEnemyDamage { get; private set; }
    public float MultipleEnemyMovementSpeed { get; private set; }
    public float MultipleEnemyAttackSpeed { get; private set; }
    public float MultipleEnemyHP { get; private set; }
    public float ProbabilityWrongTurn { get; private set; }
    public float MultipleEnemyAttackRange { get; private set; }

    public UpgradeStat()
    {
        AddPlayerDamage = 0;
        MultplePlayerProjectileSpeed = 1f;
        AddShotSameDir = 0;
        AddShotRandomDir = 0;
        AddShotBehindDir = false;
        MultiplePlayerAttackSpeed = 1f;
        MultipleTurnSpeed = 1f;
        NormalizeShotDir = false;
        AddHealPerSecond = 0;
        AddHealOnKill = 0;
        MultipleHealAmount = 1f;
        AddHP = 0;
        MultipleAppearEnemyTime = 1f;
        AddEnemyCountByWave = 0;
        StartAddEnemyCountByWave = -1;
        MultipleEnemyCount = 1f;
        MultipleEnemyDamage = 1f;
        MultipleEnemyMovementSpeed = 1f;
        MultipleEnemyAttackSpeed = 1f;
        MultipleEnemyHP = 1f;
        ProbabilityWrongTurn = 1f;
        MultipleEnemyAttackRange = 1f;
    }

    public void UpdatePlayerDamage(int value)
    {
        AddPlayerDamage = value;
    }

    public void UpdatePlayerProjectileSpeed(float value)
    {
        MultplePlayerProjectileSpeed = 1f + value;
    }

    public void UpdateShotSameDir(int value, float panelty)
    {
        AddShotSameDir = value;
        PaneltyPlayerDamage -= panelty;
    }

    public void UpdateShotRandomDir(int value, float panelty)
    {
        AddShotRandomDir = value;
        PaneltyPlayerDamage -= panelty;
    }

    public void UpdateShotBehindDir(bool value)
    {
        AddShotBehindDir = value;
    }

    public void UpdateReloadTime(float value)
    {
        MultiplePlayerAttackSpeed = 1f + value;
    }

    public void UpdateTurnSpeed(float value)
    {
        MultipleTurnSpeed = 1f - value;
    }

    public void UpdateNormalizeShotDir(bool value)
    {
        NormalizeShotDir = value;
    }

    public void UpdateHealPerSecond(int value)
    {
        AddHealPerSecond = value;
    }

    public void UpdateHealOnKill(int value)
    {
        AddHealOnKill = value;
    }

    public void UpdateHealAmount(float value)
    {
        MultipleHealAmount = 1f + value;
    }

    public void UpdateHP(int value)
    {
        AddHP = value;
    }

    public void UpdateAppearEnemyTime(float value)
    {
        MultipleAppearEnemyTime = 1f - value;
    }

    public void UpdateEnemyCountByWave(int value, int wave)
    {
        AddEnemyCountByWave = value;
        if (StartAddEnemyCountByWave < 0)
        {
            StartAddEnemyCountByWave = wave;
        }
    }

    public void UpdateMultipleEnemyCount(float value)
    {
        MultipleEnemyCount = 1f + value;
    }

    public void UpdateEnemyDamage(float value)
    {
        MultipleEnemyDamage = 1f + value;
    }

    public void UpdateEnemyMovementSpeed(float value)
    {
        MultipleEnemyMovementSpeed = 1f + value;
    }

    public void UpdateEnemyAttackSpeed(float value)
    {
        MultipleEnemyAttackSpeed = 1f - value;
    }

    public void UpdateEnemyHP(float value)
    {
        MultipleEnemyHP = 1f + value;
    }

    public void UpdateProbabilityWrongTurn(float value)
    {
        ProbabilityWrongTurn = 1f - value;
    }
    public void UpdateEnemyAttackRange(float value)
    {
        MultipleEnemyAttackRange = 1f + value;
    }
}