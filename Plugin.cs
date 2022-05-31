using BepInEx;
using System;
using UnityEngine;
using Utilla;
using UnityEngine.XR;
using System.IO;
using System.Reflection;

namespace OhioFlag
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;

        GameObject _OhioSound;

        GameObject _OhioFlag;

        GameObject HandR;

        private readonly XRNode rNode = XRNode.RightHand;

        bool isgrip;

        bool cangrip = true;

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("OhioFlag.Assets.ohiosound");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            GameObject UkraineObject = bundle.LoadAsset<GameObject>("OhioSound");
            _OhioSound = Instantiate(UkraineObject);

            Stream _str = Assembly.GetExecutingAssembly().GetManifestResourceStream("OhioFlag.Assets.ohioflag");
            AssetBundle _bundle = AssetBundle.LoadFromStream(_str);
            GameObject OhioFlagObject = _bundle.LoadAsset<GameObject>("OhioFlag");
            _OhioFlag = Instantiate(OhioFlagObject);

            HandR = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/");
            _OhioFlag.transform.SetParent(HandR.transform, false);
            _OhioFlag.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
            _OhioFlag.transform.localRotation = Quaternion.Euler(0f, 335f, 90f);
            _OhioFlag.transform.localPosition = new Vector3(-0.1f, -0.1f, -0.02f);
        }

        void Update()
        {
            InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out isgrip);

            if (isgrip)
            {
                // This is so we cannot spam the noise
                if (cangrip)
                {
                    _OhioSound.GetComponent<AudioSource>().Play();
                    cangrip = false;
                }
            }
            else
            {
                // This is where grip is not pressed so here we will make it so u can grip
                cangrip = true;
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
}