//using system.collections;
//using system.collections.generic;
//using unityengine;

//public class joystickdirectionindicator3 : monobehaviour
//{
//    [header("조이스틱 (선택 사항)")]
//    public variablejoystick joystick;

//    [header("조이스틱 투명도 조절용")]
//    public canvasgroup joystickcanvasgroup;

//    [header("조이스틱 조작 중 숨길 이미지")]
//    public gameobject imagetohidewhentouching;

//    [header("스킬 범위 스프라이트 (1~4)")]
//    public list<gameobject> directionspriteprefabs;

//    [header("스킬 범위 중앙값 설정 (1~4)")]
//    public list<float> distancesfromplayer;
//    public list<float> spritebackoffsets;

//    [header("스킬별 범위 회전 오프셋 (각도, 1~4)")]
//    public list<float> skillangleoffsets;

//    [header("파이어볼 관련")]
//    public gameobject fireballprefab;
//    public transform firepoint;

//    [header("번개 관련")]
//    public gameobject lightningprefab;
//    public gameobject lightningeffectprefab;

//    [header("바람막 관련")]
//    public gameobject windwallprefab;

//    private gameobject indicatorinstance;
//    private int currentindicatorindex = -1;

//    private playercontroller playercontroller;

//    private bool istouchingjoystick = false;
//    private bool wastouchingjoysticklastframe = false;
//    private vector2 lastinputdirection = vector2.right;
//    private float lastinputmagnitude = 0f;

//    private bool hasusedskill = false;
//    private bool previsrolling = false;

//    private bool isteleportmode = false;
//    private vector3 teleporttargetposition;

//    private bool islightningmode = false;
//    private vector3 lightningtargetposition;

//    private vector2 lightningcastdirection;


//    void start()
//    {
//        playercontroller = getcomponent<playercontroller>();
//        if (joystickcanvasgroup != null)
//            joystickcanvasgroup.alpha = 0f;
//    }

//    void update()
//    {
//        if (diceanimation.currentdiceresult <= 0)
//        {
//            joystick.enabled = false;
//            if (joystickcanvasgroup != null) joystickcanvasgroup.alpha = 0f;
//            if (imagetohidewhentouching != null) imagetohidewhentouching.setactive(true);
//            if (indicatorinstance != null) indicatorinstance.setactive(false);
//            currentindicatorindex = -1;
//            return;
//        }

//        if (previsrolling && !diceanimation.isrolling)
//        {
//            hasusedskill = false;
//            isteleportmode = false;
//            islightningmode = false;
//        }
//        previsrolling = diceanimation.isrolling;

//        int currentdiceresult = diceanimation.currentdiceresult;

//        if (hasusedskill)
//        {
//            istouchingjoystick = false;
//            if (imagetohidewhentouching != null) imagetohidewhentouching.setactive(true);
//            if (indicatorinstance != null) indicatorinstance.setactive(false);
//            currentindicatorindex = -1;
//            if (joystickcanvasgroup != null) joystickcanvasgroup.alpha = 0f;
//            joystick.enabled = false;
//            return;
//        }

//        vector2 input = (joystick != null) ? new vector2(joystick.horizontal, joystick.vertical) : playercontroller.inputvector;
//        istouchingjoystick = input.magnitude > 0.2f;

//        if (joystickcanvasgroup != null)
//            joystickcanvasgroup.alpha = istouchingjoystick ? 1f : 0f;

//        if (istouchingjoystick)
//        {
//            lastinputdirection = input.normalized;
//            lastinputmagnitude = input.magnitude;

//            if (currentdiceresult == 2 && isteleportmode)
//            {
//                updateteleportindicator(input);
//            }
//            else if (currentdiceresult == 3 && islightningmode)
//            {
//                updatelightningindicator(input);
//            }
//            else
//            {
//                onskillbuttonpressed();
//                updateskillindicator(input, currentdiceresult);
//            }
//        }
//        else
//        {
//            if (wastouchingjoysticklastframe && !hasusedskill && lastinputmagnitude > 0.3f)
//            {
//                onskillbuttonreleased();
//                hasusedskill = true;
//            }

//            if (imagetohidewhentouching != null) imagetohidewhentouching.setactive(true);
//            if (indicatorinstance != null) indicatorinstance.setactive(false);
//            currentindicatorindex = -1;
//        }

//        wastouchingjoysticklastframe = istouchingjoystick;
//        joystick.enabled = !diceanimation.isrolling && !hasusedskill;
//    }

//    void updateskillindicator(vector2 input, int currentdiceresult)
//    {
//        if (imagetohidewhentouching != null)
//            imagetohidewhentouching.setactive(false);

//        if (currentdiceresult >= 1 && currentdiceresult <= directionspriteprefabs.count)
//        {
//            int index = currentdiceresult - 1;

//            if (currentindicatorindex != index)
//            {
//                if (indicatorinstance != null) destroy(indicatorinstance);
//                indicatorinstance = instantiate(directionspriteprefabs[index], transform.position, quaternion.identity);
//                currentindicatorindex = index;
//            }

//            if (!indicatorinstance.activeself)
//                indicatorinstance.setactive(true);

//            vector3 direction = new vector3(input.x, input.y, 0f).normalized;
//            float angle = mathf.atan2(direction.y, direction.x) * mathf.rad2deg;
//            float offsetangle = (skillangleoffsets != null && skillangleoffsets.count > index) ? skillangleoffsets[index] : 0f;
//            indicatorinstance.transform.rotation = quaternion.euler(0f, 0f, angle + offsetangle);

//            float dist = (distancesfromplayer != null && distancesfromplayer.count > index) ? distancesfromplayer[index] : 0f;
//            float backoffset = (spritebackoffsets != null && spritebackoffsets.count > index) ? spritebackoffsets[index] : 0f;

//            vector3 basepos = transform.position + direction * dist;
//            vector3 offset = -indicatorinstance.transform.up * backoffset;
//            indicatorinstance.transform.position = basepos + offset;
//        }
//        else
//        {
//            if (indicatorinstance != null)
//                indicatorinstance.setactive(false);
//            currentindicatorindex = -1;
//        }
//    }

//    void updateteleportindicator(vector2 input)
//    {
//        float maxdist = (distancesfromplayer != null && distancesfromplayer.count > 1) ? distancesfromplayer[1] : 5f;
//        float inputmagnitudeclamped = mathf.clamp01(input.magnitude);
//        vector3 direction = new vector3(input.x, input.y, 0f).normalized;
//        vector3 basepos = transform.position + direction * maxdist * inputmagnitudeclamped;

//        teleporttargetposition = basepos;

//        if (indicatorinstance != null)
//        {
//            indicatorinstance.setactive(true);
//            float backoffset = (spritebackoffsets != null && spritebackoffsets.count > 1) ? spritebackoffsets[1] : 0f;
//            indicatorinstance.transform.position = basepos - indicatorinstance.transform.up * backoffset;

//            float angle = mathf.atan2(direction.y, direction.x) * mathf.rad2deg;
//            float offsetangle = (skillangleoffsets != null && skillangleoffsets.count > 1) ? skillangleoffsets[1] : 0f;
//            indicatorinstance.transform.rotation = quaternion.euler(0f, 0f, angle + offsetangle);
//        }
//    }

//    void updatelightningindicator(vector2 input)
//    {
//        float maxdist = (distancesfromplayer != null && distancesfromplayer.count > 2) ? distancesfromplayer[2] : 5f;
//        float inputmagnitudeclamped = mathf.clamp01(input.magnitude);
//        vector3 direction = new vector3(input.x, input.y, 0f).normalized;
//        vector3 basepos = transform.position + direction * maxdist * inputmagnitudeclamped;

//        lightningtargetposition = basepos;

//        if (indicatorinstance != null)
//        {
//            indicatorinstance.setactive(true);
//            float backoffset = (spritebackoffsets != null && spritebackoffsets.count > 2) ? spritebackoffsets[2] : 0f;
//            indicatorinstance.transform.position = basepos - indicatorinstance.transform.up * backoffset;

//            float angle = mathf.atan2(direction.y, direction.x) * mathf.rad2deg;
//            float offsetangle = (skillangleoffsets != null && skillangleoffsets.count > 2) ? skillangleoffsets[2] : 0f;
//            indicatorinstance.transform.rotation = quaternion.euler(0f, 0f, angle + offsetangle);
//        }
//    }

//    public void onskillbuttonpressed()
//    {
//        if (joystickcanvasgroup != null)
//            joystickcanvasgroup.alpha = 1f;

//        int currentdiceresult = diceanimation.currentdiceresult;

//        if (currentdiceresult == 2)
//        {
//            isteleportmode = true;
//            setupindicator(1);
//        }
//        else if (currentdiceresult == 3)
//        {
//            islightningmode = true;
//            setupindicator(2);
//        }
//    }

//    void setupindicator(int prefabindex)
//    {
//        if (currentindicatorindex != prefabindex)
//        {
//            if (indicatorinstance != null) destroy(indicatorinstance);

//            if (directionspriteprefabs.count > prefabindex)
//            {
//                indicatorinstance = instantiate(directionspriteprefabs[prefabindex], transform.position, quaternion.identity);
//                currentindicatorindex = prefabindex;
//            }
//        }
//    }

//    public void onskillbuttonreleased()
//    {
//        if (joystickcanvasgroup != null)
//            joystickcanvasgroup.alpha = 0f;

//        int currentdiceresult = diceanimation.currentdiceresult;

//        switch (currentdiceresult)
//        {
//            case 1:
//                shootfireball();
//                break;
//            case 2:
//                if (isteleportmode)
//                {
//                    teleportplayer(teleporttargetposition);
//                    isteleportmode = false;
//                }
//                break;
//            case 3:
//                if (islightningmode)
//                {
//                    lightningcastdirection = lastinputdirection;  // <<< 시전 방향 저장
//                    castlightning(lightningtargetposition);
//                    islightningmode = false;
//                }
//                break;

//            case 4:
//                spawnwindwall();
//                break;
//            default:
//                debug.log("해당 스킬은 아직 구현되지 않았습니다.");
//                break;
//        }

//        debug.log("스킬 발사!!");
//    }

//    private void shootfireball()
//    {
//        if (fireballprefab == null || firepoint == null)
//        {
//            debug.logwarning("fireball prefab or firepoint not assigned.");
//            return;
//        }

//        gameobject fireballobj = instantiate(fireballprefab, firepoint.position, quaternion.identity);
//        vector2 shootdir = lastinputdirection;
//        float angle = mathf.atan2(shootdir.y, shootdir.x) * mathf.rad2deg;
//        fireballobj.transform.rotation = quaternion.euler(0f, 0f, angle);

//        fireballprojectile fireball = fireballobj.getcomponent<fireballprojectile>();
//        if (fireball != null)
//        {
//            fireball.init(shootdir);
//        }
//    }

//    private void teleportplayer(vector3 targetpos)
//    {
//        transform.position = targetpos;
//        debug.log($"플레이어 텔레포트: {targetpos}");
//    }

//    private void castlightning(vector3 targetpos)
//    {
//        startcoroutine(lightningstrikesequence(targetpos));
//    }

//    private ienumerator lightningstrikesequence(vector3 targetpos)
//    {
//        int strikecount = 3;
//        float strikeonduration = 0.2f;   // 켜져 있는 시간
//        float strikeoffduration = 0.1f;  // 꺼져 있는 시간

//        vector2 direction = lightningcastdirection.normalized;
//        float angle = mathf.atan2(direction.y, direction.x) * mathf.rad2deg;

//        gameobject lightning = null;
//        lightningdamage ld = null;

//        gameobject lightningeffect = null;

//        for (int i = 0; i < strikecount; i++)
//        {
//            // lightningprefab 깜빡임
//            if (lightning == null && lightningprefab != null)
//            {
//                lightning = instantiate(lightningprefab, targetpos, quaternion.euler(0f, 0f, angle));
//                ld = lightning.getcomponent<lightningdamage>();
//            }
//            else if (lightning != null)
//            {
//                lightning.transform.position = targetpos;
//                lightning.transform.rotation = quaternion.euler(0f, 0f, angle);
//                lightning.setactive(true);
//            }

//            if (ld != null)
//                ld.init();

//            // lightningeffectprefab 깜빡임
//            if (lightningeffect == null && lightningeffectprefab != null)
//            {
//                vector3 effectpos = targetpos + vector3.up * 2f;
//                lightningeffect = instantiate(lightningeffectprefab, effectpos, quaternion.identity);
//            }
//            else if (lightningeffect != null)
//            {
//                lightningeffect.transform.position = targetpos + vector3.up * 2f;
//                lightningeffect.setactive(true);
//            }

//            yield return new waitforseconds(strikeonduration);

//            if (lightning != null)
//                lightning.setactive(false);

//            if (lightningeffect != null)
//                lightningeffect.setactive(false);

//            yield return new waitforseconds(strikeoffduration);
//        }

//        if (lightning != null)
//            destroy(lightning);

//        if (lightningeffect != null)
//            destroy(lightningeffect);
//    }


//    private void spawnwindwall()
//    {
//        if (windwallprefab == null)
//        {
//            debug.logwarning("windwall prefab not assigned.");
//            return;
//        }

//        gameobject wall = instantiate(windwallprefab, transform.position, quaternion.identity);

//        vector2 spawndir = lastinputdirection;
//        if (spawndir == vector2.zero)
//            spawndir = vector2.right;

//        float angle = mathf.atan2(spawndir.y, spawndir.x) * mathf.rad2deg;
//        wall.transform.rotation = quaternion.euler(0f, 0f, angle);

//        debug.log($"바람막 생성 at {transform.position}, angle {angle}");
//    }
//}



//using UnityEngine;

//private IEnumerator LightningStrikeSequence()
//{
//    int skillCase = 3;
//    float dist = (skillSelect != null) ? skillSelect.GetDistanceForSkillCase(skillCase) : 5f;

//    Vector3 direction = lastInputDirection.normalized;
//    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

//    // 고정 위치로 한 번만 계산!
//    Vector3 fixedTargetPos = transform.position + direction * dist;

//    int strikeCount = 3;
//    float strikeOnDuration = 0.2f;
//    float strikeOffDuration = 0.1f;

//    for (int i = 0; i < strikeCount; i++)
//    {
//        GameObject lightning = null;
//        LightningDamage ld = null;

//        if (lightningPrefab != null)
//        {
//            lightning = Instantiate(lightningPrefab, fixedTargetPos, Quaternion.Euler(0f, 0f, angle));
//            ld = lightning.GetComponent<LightningDamage>();
//            if (ld != null) ld.Init();
//        }

//        GameObject lightningEffect = null;
//        if (LightningEffectPrefab != null)
//        {
//            Vector3 startPos = fixedTargetPos + Vector3.up * 5f;
//            lightningEffect = Instantiate(LightningEffectPrefab, startPos, Quaternion.identity);

//            FallingLightningEffect fle = lightningEffect.GetComponent<FallingLightningEffect>();
//            if (fle != null)
//            {
//                fle.targetPosition = fixedTargetPos;
//            }
//        }

//        yield return new WaitForSeconds(strikeOnDuration);

//        if (lightning != null) Destroy(lightning);
//        if (lightningEffect != null) Destroy(lightningEffect);

//        yield return new WaitForSeconds(strikeOffDuration);
//    }
//}