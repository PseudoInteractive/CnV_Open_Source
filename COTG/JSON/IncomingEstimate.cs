﻿using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Game.Enum;

namespace COTG.Game
{
    public static class IncomingEstimate
    {
        public static float RoundTo6Bits(double f)
        {
            return (float)(Math.Round(f * 100)/100.0);

        }
        static float StepValue(int i) => i * 0.5f;
        //        static int StepI(int i) => i/2;

        public unsafe static void Get(Army army)
        {
  //          if(Player.myName == "Avatar" )
            {
                var source = Spot.GetOrAdd(army.sourceCid);
                if (source.isClassified)
                {
                    switch (source.classification)
                    {
                        case Spot.Classification.unknown:
                            break;
                        case Spot.Classification.vanqs:
                            army.troops = new[] { new TroopTypeCount(ttVanquisher, -1) };
                            return;
                        case Spot.Classification.rt:
                            army.troops = new[] { new TroopTypeCount(ttRanger, -1) };
                            return;
                        case Spot.Classification.sorcs:
                            army.troops = new[] { new TroopTypeCount(ttSorcerer, -1) };
                            return;

                        case Spot.Classification.druids:
                            army.troops = new[] { new TroopTypeCount(ttDruid, -1) };
                            return;
                        case Spot.Classification.academy:
                            army.troops = new[] { new TroopTypeCount(ttPraetor, -1) };
                            return;
                        case Spot.Classification.horses:
                            army.troops = new[] { new TroopTypeCount(ttHorseman, -1) };
                            return;
                        case Spot.Classification.arbs:
                            army.troops = new[] { new TroopTypeCount(ttArbalist, -1) };
                            return;
                        case Spot.Classification.se:
                            army.troops = new[] { new TroopTypeCount(ttScorpion, -1) };
                            return;

                        case Spot.Classification.navy:
                            army.troops = new[] { new TroopTypeCount(ttWarship, -1) };
                            return;

                    }
                }
                else
                {
                    source.Classify();
                }

            }

            const int steps = 201;
            Span<float> navyspeed_ = stackalloc float[steps];
            Span<float> scoutspeed_ = stackalloc float[steps];
            Span<float> cavspeed_ = stackalloc float[steps];
            Span<float> infspeed_ = stackalloc float[steps];
            Span<float> artspeed_ = stackalloc float[steps];
            Span<float> senspeed_ = stackalloc float[steps];
            List<TroopTypeCount> rv = new List<TroopTypeCount>();
            for (int i_17 = 0; i_17 < steps; ++i_17)
            {
                /** @type {number} */
                var temp_1 = 5*100 / (100 + StepValue(i_17));
                navyspeed_[i_17] = RoundTo6Bits(temp_1);
                /** @type {number} */
                temp_1 = 8*100 / (100 + StepValue(i_17));
                scoutspeed_[i_17] = RoundTo6Bits(temp_1);
                /** @type {number} */
                temp_1 = 10 * 100 / (100 + StepValue(i_17));
                cavspeed_[i_17] = RoundTo6Bits(temp_1);
                /** @type {number} */
                temp_1 = 20 * 100 / (100 + StepValue(i_17));
                infspeed_[i_17] = RoundTo6Bits(temp_1);
                /** @type {number} */
                temp_1 = 30 * 100 / (100 + StepValue(i_17));
                artspeed_[i_17] = RoundTo6Bits(temp_1);
                /** @type {number} */
                temp_1 = 40 * 100 / (100 + StepValue(i_17));
                senspeed_[i_17] = RoundTo6Bits(temp_1);
            }
            var targetContinent = army.targetCid.CidToContinent();
            var sourceContinent = army.sourceCid.CidToContinent();
            //$("#iaBody tr").each(function() {
            //              const tid_2 = GetIntData($(":nth-child(5)", this).children().children());
            //              const sid_ = GetIntData($(":nth-child(10)", this).children());
            //              /** @type {number} */
            //              const tx_ = tid_2 % 65536;
            //              /** @type {number} */
            //              const sx_1 = sid_ % 65536;
            //              /** @type {number} */
            //              const ty_ = (tid_2 - tx_) / 65536;
            //              /** @type {number} */
            //              const sy_1 = (sid_ - sx_1) / 65536;
            //              /** @type {number} */
            //              const tcont_2 = Math.floor(tx_ / 100) + Math.floor(ty_ / 100) * 10;
            //              /** @type {number} */
            //              const scont_ = Math.floor(sx_1 / 100) + Math.floor(sy_1 / 100) * 10;
            //              /** @type {number} */
            //              const dist_ = Math.sqrt((ty_ - sy_1) * (ty_ - sy_1) + (tx_ - sx_1) * (tx_ - sx_1));
            //              const atime_ = $(":nth-child(6)", this).text();
            //              const stime_ = $(":nth-child(11)", this).text();
            //              /** @type {number} */
            //              let hdiff_ = parseInt(atime_.substring(0, 2)) - parseInt(stime_.substring(0, 2));
            //              /** @type {number} */
            //              let mdiff_ = parseInt(atime_.substring(3, 5)) - parseInt(stime_.substring(3, 5));
            //              /** @type {number} */
            //              const sdiff_ = parseInt(atime_.substring(6, 8)) - parseInt(stime_.substring(6, 8));
            //              /** @type {!Date} */
            //              const d_3 = new Date;
            //              let arrivaltimemonth_;
            //              let arrivaltimedate_;
            //              if (atime_.length === 14)
            //              {
            //                  /** @type {number} */
            //                  arrivaltimemonth_ = AsNumber(atime_.substring(9, 11));
            //                  /** @type {number} */
            //                  arrivaltimedate_ = AsNumber(atime_.substring(12, 14));
            //              }
            //              else
            //              {
            //                  /** @type {number} */
            //                  arrivaltimemonth_ = d_3.getMonth() + 1;
            //                  /** @type {number} */
            //                  arrivaltimedate_ = d_3.getDate();
            //              }
            //              let time_4;
            //              if (hdiff_ >= 0)
            //              {
            //                  /** @type {number} */
            //                  time_4 = 60 * hdiff_;
            //              }
            //              else
            //              {
            //                  /** @type {number} */
            //                  time_4 = (24 + hdiff_) * 60;
            //              }
            //              if ((atime_.length === 14 || stime_.length === 14) && hdiff_ > 0)
            //              {
            //                  /** @type {number} */
            //                  time_4 = time_4 + +1440;
            //                  /** @type {number} */
            //                  hdiff_ = hdiff_ + 24;
            //              }
            //              /** @type {number} */
            //              time_4 = time_4 + mdiff_;
            //              /** @type {number} */
            //              time_4 = time_4 + sdiff_ / 60;
            var journeyTime = army.journeyTime / 60.0; // want minutes
            var dist_ = army.dist;
            float landSpeed = RoundTo6Bits(journeyTime / (double)dist_);

            float navySpeed = RoundTo6Bits((journeyTime - 60) / (double)dist_);
            //            let locks_;
            //            let lockm_;
            //            let lockh_;
            //            if (sdiff_ >= 0)
            //            {
            //                /** @type {number} */
            //                locks_ = sdiff_;
            //            }
            //            else
            //            {
            //                /** @type {number} */
            //                locks_ = 60 + sdiff_;
            //                /** @type {number} */
            //                mdiff_ = mdiff_ - 1;
            //            }
            //            if (mdiff_ >= 0)
            //            {
            //                /** @type {number} */
            //                lockm_ = mdiff_;
            //            }
            //            else
            //            {
            //                /** @type {number} */
            //                lockm_ = 60 + mdiff_;
            //                /** @type {number} */
            //                hdiff_ = hdiff_ - 1;
            //            }
            //            if (hdiff_ >= 0)
            //            {
            //                /** @type {number} */
            //                lockh_ = hdiff_;
            //            }
            //            else
            //            {
            //                /** @type {number} */
            //                lockh_ = hdiff_ + 24;
            //            }
            //            const travelingts_ = TwoDigitNum(locks_);
            //            const travelingtm_ = TwoDigitNum(lockm_);
            //            const travelingth_ = TwoDigitNum(lockh_);
            //            /** @type {number} */
            //            let locktimeh_ = AsNumber(lockh_) + AsNumber(atime_.substring(0, 2));
            //            /** @type {number} */
            //            let locktimem_ = AsNumber(lockm_) + AsNumber(atime_.substring(3, 5));
            //            /** @type {number} */
            //            let locktimes_ = AsNumber(locks_) + AsNumber(atime_.substring(6, 8));
            //            if (locktimes_ > 59)
            //            {
            //                /** @type {number} */
            //                locktimes_ = locktimes_ - 60;
            //                /** @type {number} */
            //                locktimem_ = locktimem_ + 1;
            //            }
            //            if (locktimem_ > 59)
            //            {
            //                /** @type {number} */
            //                locktimem_ = locktimem_ - 60;
            //                /** @type {number} */
            //                locktimeh_ = locktimeh_ + 1;
            //            }
            //            if (locktimeh_ > 23)
            //            {
            //                /** @type {number} */
            //                locktimeh_ = locktimeh_ - 24;
            //                /** @type {number} */
            //                arrivaltimedate_ = arrivaltimedate_ + 1;
            //            }
            //            /** @type {!Array} */
            //            const atm1_ = [1, 3, 5, 7, 8, 10, 12];
            //            /** @type {!Array} */
            //            const atm2_ = [4, 6, 9, 11];
            //            if (atm1_.indexOf(arrivaltimemonth_) > 0)
            //            {
            //                if (arrivaltimedate_ > 31)
            //                {
            //                    /** @type {number} */
            //                    arrivaltimedate_ = 1;
            //                }
            //            }
            //            if (atm2_.indexOf(arrivaltimemonth_) > 0)
            //            {
            //                if (arrivaltimedate_ > 30)
            //                {
            //                    /** @type {number} */
            //                    arrivaltimedate_ = 1;
            //                }
            //            }
            //            if (arrivaltimemonth_ === 2)
            //            {
            //                if (arrivaltimedate_ > 28)
            //                {
            //                    /** @type {number} */
            //                    arrivaltimedate_ = 1;
            //                }
            //            }
            //            const addt_ = $(this);
            //            arrivaltimemonth_ = TwoDigitNum(arrivaltimemonth_);
            //            arrivaltimedate_ = TwoDigitNum(arrivaltimedate_);
            //            /** @type {string} */
            //            const newtd_ = "<td></td>";
            //            if (addt_.children().length === 14)
            //            {
            //$(this).append(newtd_);
            //$(":nth-child(15)", this).text(`${ TwoDigitNum(locktimeh_)}:${ TwoDigitNum(locktimem_)}:${ TwoDigitNum(locktimes_)} ${ arrivaltimemonth_}/${ arrivaltimedate_}`);
            //                if ($(":nth-child(2)", this).text() == "Sieging") {
            //	$(":nth-child(15)", this).css("color", "red");
            //                }
            //            }
            //            if (addt_.children().length === 15)
            //            {
            //$(this).append(newtd_);
            //$(":nth-child(16)", this).text(`${ travelingth_}:${ travelingtm_}:${ travelingts_}`);
            //                if ($(":nth-child(2)", this).text() == "Sieging") {
            //	$(":nth-child(16)", this).css("color", "red");
            //                }
            //            }
            //  if ($(":nth-child(2)", this).text() == "-") {
            /** @type {number} */
            var zns_ = navyspeed_.IndexOf(navySpeed);
            /** @type {number} */
            var zss_ = scoutspeed_.IndexOf(landSpeed);
            /** @type {number} */
            var zcs_ = cavspeed_.IndexOf(landSpeed);
            /** @type {number} */
            var zis_ = infspeed_.IndexOf(landSpeed);
            /** @type {number} */
            var zas_ = artspeed_.IndexOf(landSpeed);
            /** @type {number} */
            var zsn_ = senspeed_.IndexOf(landSpeed);
            const int tsGain = -5;
            if (targetContinent == sourceContinent)
            {
                if (landSpeed > 30)
                {
                    if (zsn_ == -1)
                    {
                        //							rv.Add(new TroopTypeCount(ttSen");
                        rv.Add(new TroopTypeCount(ttSenator, -1));
                    }
                    else
                    {
                        //							rv.Add(new TroopTypeCount(ttsenator , 5*zsn_));
                        rv.Add(new TroopTypeCount(ttSenator, zsn_ * tsGain));
                    }
                }
                if (landSpeed > 20 && landSpeed <= 30)
                {
                    if (zsn_ == -1 && zas_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttRam, -1));
                        rv.Add(new TroopTypeCount(ttSenator, -1));

                    }
                    if (zsn_ == -1 && zas_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttRam, zas_ * tsGain));
                    }
                    if (zsn_ != -1 && zas_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttSenator, zsn_ * tsGain));
                    }
                    if (zsn_ != -1 && zas_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttRam, zas_ * tsGain));
                        rv.Add(new TroopTypeCount(ttSenator, zsn_ * tsGain));
                    }
                }
                if (landSpeed == 20)
                {
                    rv.Add(new TroopTypeCount(ttVanquisher, 0));
                    rv.Add(new TroopTypeCount(ttRam, 500));
                    rv.Add(new TroopTypeCount(ttSenator, 1000));
                    //						$(":nth-child(2)", this).text("Inf 0%/Art 50%/Sen 100%");
                }
                if (landSpeed >= 15 && landSpeed < 20)
                {
                    if (zis_ == -1 && zas_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttVanquisher, -1));
                    }
                    if (zis_ == -1 && zas_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttRam, tsGain * zas_));
                    }
                    if (zis_ != -1 && zas_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttVanquisher, tsGain * zis_));
                    }
                    if (zis_ != -1 && zas_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttVanquisher, tsGain * zis_));
                        rv.Add(new TroopTypeCount(ttRam, tsGain * zas_));
                    }
                }
                if (landSpeed >= 10 && landSpeed < 15)
                {
                    if (zis_ == -1 && zcs_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttHorseman, -1));
                    }
                    if (zis_ == -1 && zcs_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttHorseman, tsGain * zcs_));
                    }
                    if (zis_ != -1 && zcs_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttVanquisher, tsGain * zis_));
                    }
                    if (zis_ != -1 && zcs_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttHorseman, tsGain * zcs_));
                        rv.Add(new TroopTypeCount(ttVanquisher, tsGain * zis_));
                    }
                }
                if (landSpeed > 8 && landSpeed < 10)
                {
                    if (zcs_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttHorseman, -1));
                    }
                    else
                    {
                        rv.Add(new TroopTypeCount(ttHorseman, tsGain * zcs_));
                    }
                }
                if (landSpeed > 5 && landSpeed <= 8)
                {
                    if (zss_ == -1 && zcs_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttScout, -1));
                    }
                    if (zss_ == -1 && zcs_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttHorseman, tsGain * zcs_));
                    }
                    if (zss_ != -1 && zcs_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttScout, tsGain * zss_));
                    }
                    if (zss_ != -1 && zcs_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttScout, tsGain * zss_));
                        rv.Add(new TroopTypeCount(ttHorseman, tsGain * zcs_));
                    }
                }
                if (landSpeed == 5)
                {
                    rv.Add(new TroopTypeCount(ttWarship, 0));
                    rv.Add(new TroopTypeCount(ttScout, 600));
                    rv.Add(new TroopTypeCount(ttHorseman, 1000));
                }
                if (landSpeed >= 4 && landSpeed < 5)
                {
                    if (zss_ == -1 && zns_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttScout, -1));
                    }
                    if (zss_ == -1 && zns_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttWarship, tsGain * zns_));
                    }
                    if (zss_ != -1 && zns_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttScout, tsGain * zss_));
                    }
                    if (zss_ != -1 && zns_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttWarship, tsGain * zns_));
                        rv.Add(new TroopTypeCount(ttScout, tsGain * zss_));
                    }
                }
                if (landSpeed < 4)
                {
                    if (zns_ == -1)
                    {
                        rv.Add(new TroopTypeCount(ttWarship, -1));
                    }
                    else
                    {
                        rv.Add(new TroopTypeCount(ttWarship, tsGain * zns_));
                    }
                }
            }
            else
            {
                //                       if(army.por $(":nth-child(1)", this).html()) {
                ////					$(":nth-child(2)", this).text("Portal");
                //                       }
                //                       else
                {
                    if (zns_ != -1)
                    {
                        rv.Add(new TroopTypeCount(ttWarship, tsGain * zns_));
                    }
                    else
                    {
                        rv.Add(new TroopTypeCount(ttWarship, -1));
                    }
                }
            }
            army.troops = rv.ToArray();
        }
    }
}

