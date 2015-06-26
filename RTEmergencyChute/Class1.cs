/**
 * RTEmergencyChute
 * A KSP + RemoteTech Addon.
 * Written by Ricky Hewitt
 * 
 * (C) Copyright 2015, Ricky Hewitt
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 * Licensed under Apache License Version 2.0
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RtAPI = RemoteTech.API.API;

namespace RTEmergencyChute
{
    public class RTEmergencyChute : PartModule
    {
        private string deploySafe;
        private float deployAltitude;
        private float minAirPressureToOpen;

        /*
        * Caution: as it says here: http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.Awake.html,
        * use the Awake() method instead of the constructor for initializing data because Unity uses
        * Serialization a lot.
        */
        public RTEmergencyChute()
        {
        }

        /*
        * Called at a fixed time interval determined by the physics time step.
        * Unlike OnFixedUpdate, this will run regardless if part is active or not.
        * 30 times a second?
        */
        public void FixedUpdate()
        {
            /* 
                Get ModuleParachute values
                There is probably a better way to do this.. I'm kinda new to C# and Unity.
                If you know how, please let me know! 
            */
            foreach (ModuleParachute mP in part.Modules.GetModules<ModuleParachute>())
            {
                //print("mP <" + mP.GetInstanceID().ToString() + ">: " + mP.moduleName + "");
                deploySafe = mP.deploySafe;
                minAirPressureToOpen = mP.minAirPressureToOpen;
                deployAltitude = mP.deployAltitude;
            }

            // We only want to deploy a chute on the non-responsive craft if we're in atmosphere (flying)
            if (vessel.situation == Vessel.Situations.FLYING && this.isEnabled)
            {
                // Check if we have lost RT connection (and that parachute has not been staged)
                if (RtAPI.HasAnyConnection(vessel.id) == false && this.part.State == PartStates.IDLE)
                {
                    //Debug.Log("RTEmergencyChute: Lost connection..");
                    //Debug.Log("RTEmergencyChute: Vessel verticalSpeed: " + this.vessel.verticalSpeed.ToString());
                    //Debug.Log("RTEmergencyChute: Vessel altitude: " + this.vessel.altitude.ToString());
                    //Debug.Log("RTEmergencyChute: getStaticPressure: " + FlightGlobals.getStaticPressure());

                    if (FlightGlobals.getStaticPressure() >= minAirPressureToOpen)
                    {
                        //Debug.Log("RTEmergencyChute: Within pressure conditions...");
                        /* 
                        We need to actual check if we're headed towards the surface here.
                        Otherwise it'll deploy on launch!
                        Also check altitude. 
                        We don't want to deploy when vertical speed is low when we're high up...
                        otherwise the chute will deploy, and then burnup. Deploy much lower.
                        Stock KSP max chute deployment ALTITUDE is 5000. 
                        */
                        if ((this.vessel.verticalSpeed > -350) && (this.vessel.verticalSpeed < 1)
                            && this.vessel.altitude < 6000)
                        {
                            //Debug.Log("  RTEmergencyChute: Within verticalSpeed criteria...");
                            Debug.Log("RTEmergencyChute: " + deploySafe);
                            if (deploySafe == "Safe")
                            {
                                //Debug.Log("  RTEmergencyChute: CONDITIONS MET. DEPLOYING.");
                                this.part.force_activate();
                            }
                        }
                    }
                }
            }
        }

        /*
        * Called at a fixed time interval determined by the physics time step.
        * ! Only when part is active !
        */
        public override void OnFixedUpdate()
        {
        }

    }
}
