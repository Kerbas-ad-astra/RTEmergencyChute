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

        // For debug logging
        private float lastUpdate = 0.0f;
        private float updateInterval = 1.0f;
        private List<string> debugLog = new List<string>();

        /*
        * Caution: as it says here: http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.Awake.html,
        * use the Awake() method instead of the constructor for initializing data because Unity uses
        * Serialization a lot.
        */
        public RTEmergencyChute()
        {
        }

        /*
            Called every frame
        */
        public override void OnUpdate()
        {
            // Instead of calling every frame
            // Call every defined interval
            if ((Time.time - lastUpdate) > updateInterval)
            {

                lastUpdate = Time.time;
                debugLog.Add("OnUpdate");

                /* 
                    Get ModuleParachute values
                */
                foreach (ModuleParachute mP in part.Modules.GetModules<ModuleParachute>())
                {
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
                        debugLog.Add("RTEmergencyChute: No connection..");
                        //debugLog.Add("RTEmergencyChute: Vessel verticalSpeed: " + this.vessel.verticalSpeed.ToString());
                        //debugLog.Add("RTEmergencyChute: Vessel altitude: " + this.vessel.altitude.ToString());
                        //debugLog.Add("RTEmergencyChute: getStaticPressure: " + FlightGlobals.getStaticPressure());

                        if (FlightGlobals.getStaticPressure() >= minAirPressureToOpen)
                        {
                            debugLog.Add("RTEmergencyChute: Within pressure conditions...");
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
                                debugLog.Add("RTEmergencyChute: " + deploySafe);
                                if (deploySafe == "Safe")
                                {
                                    debugLog.Add("  RTEmergencyChute: CONDITIONS MET. DEPLOYING.");
                                    this.part.force_activate();
                                }
                            }
                        }
                    }
                }

                // Output any debug messages
                foreach (string logLine in debugLog)
                {
                    Debug.Log("RTEmergencyChute [" + this.GetInstanceID().ToString("X")
                    + "][" + Time.time.ToString("0.0000") + "]: " + logLine);
                }

                // Clear the debug buffer
                debugLog.Clear();
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
