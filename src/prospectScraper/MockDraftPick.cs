using CsvHelper;
using CsvHelper.Configuration;
using prospectScraper.Maps;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace prospectScraper
{
    public class MockDraftPick
    {
        public int round;
        public string teamCity;
        public string pickNumber;
        public string playerName;
        public string school;
        public string position;
        public string reachValue;
        public int leagifyPoints;
        public string pickDate;
        public string state;


        public MockDraftPick(){}
        public MockDraftPick(string pick, string team, string name, string school, string pos, string relativeVal, string pickDate)
        {
            this.pickNumber = pick;
            this.teamCity = team;
            this.round = ConvertPickToRound(pick);
            this.playerName = name;
            this.school = school;
            this.position = pos;
            this.reachValue = relativeVal;
            this.leagifyPoints = convertPickToPoints(pick, this.round);
            this.pickDate = pickDate;
            this.state = GetState(school);
        }
        public static int ConvertPickToRound(string pick)
        {
            // Compensatory picks added on 2/1/20
            int intpick = 0;
            var canParse = int.TryParse(pick, out intpick);
            if (canParse)
            {
                /* 
                    Pick numbers without comp picks:
                    Picks 1-32 : Round 1
                    Picks 33-64: Round 2
                    Picks 65-96: Round 3
                    Picks 97-128: Round 4
                    Picks 129-159: Round 5
                    Picks 160-191: Round 6
                    Picks 192-223: Round 7

                    Pick numbers with comp picks:
                    Round 1 = picks 1-32
                    Round 2 = picks 33-64 
                    Round 3 = picks 65-103
                    Round 4 = picks 104-146
                    Round 5 = picks 147-179
                    Round 6 = picks 180-214
                    Round 7 = picks 215-255
                */
                if(intpick >= 1 && intpick <= 32)
                {
                    return 1;
                } else if (intpick >= 33 && intpick <= 64)
                {
                    return 2;
                } else if (intpick >= 65 && intpick <=103)
                {
                    return 3;
                } else if (intpick >= 104 && intpick <= 146)
                {
                    return 4;
                } else if (intpick >= 147 && intpick <= 179)
                {
                    return 5;
                } else if (intpick >= 180 && intpick <= 214)
                {
                    return 6;
                } else if (intpick >= 215 && intpick <= 255)
                {
                    return 7;
                }
                return 0;
            }
            else
            {
                return 0;
            }
            
        }
        public static int convertPickToPoints(string pick, int round)
        {
            int intpick = 0;
            var canParse = int.TryParse(pick, out intpick);
            if (canParse)
            {
                /* 
                    Top Pick: 40 Points
                    Picks 2-10: 35 Points
                    Picks 11-20: 30 Points
                    Picks 21-32: 25 Points
                    Picks 33-48: 20 Points
                    Picks 49-64: 15 Points
                    Round 3: 10 Points
                    Round 4: 8 Points
                    Round 5: 7 Points
                    Round 6: 6 Points
                    Round 7: 5 Points
                */
                if(intpick == 1)
                {
                    return 40;
                }
                else if (intpick >= 2 && intpick <= 10)
                {
                    return 35;
                }
                else if (intpick >= 11 && intpick <= 20)
                {
                    return 30;
                }
                else if (intpick >= 21 && intpick <= 32)
                {
                    return 25;
                }
                else if (intpick >= 33 && intpick <= 48)
                {
                    return 20;
                }
                else if (intpick >= 49 && intpick <= 64)
                {
                    return 15;
                } 
                else if (round == 3)
                {
                    return 10;
                } 
                else if (round == 4)
                {
                    return 8;
                } 
                else if (round == 5)
                {
                    return 7;
                } 
                else if (round == 6)
                {
                    return 6;
                } 
                else if (round == 7)
                {
                    return 5;
                }
            }
            return 0;
        }

        public static string GetState(string school)
        {
            // Get Schools and the States where they are located.
            List<School> schoolsAndConferences;
            using (var reader = new StreamReader($"info{Path.DirectorySeparatorChar}SchoolStatesAndConferences.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                csv.Configuration.RegisterClassMap<SchoolCsvMap>();
                schoolsAndConferences = csv.GetRecords<School>().ToList();
            }
            var stateResult = from s in schoolsAndConferences
                                 where s.schoolName == school
                                 select s.state;

            var srfd = stateResult.FirstOrDefault();
            string sr = "";

            if (srfd != null)
            {
                sr = srfd.ToString();
            }
            else
            {
                Console.WriteLine("Error matching school!");
            }
            

            if(sr.Length > 0)
            {
                return sr;
            }
            else
            {
                return string.Empty;
            }
            //return stateResult.FirstOrDefault().ToString();
        }
    }

    public sealed class MockDraftPickCsvMap : ClassMap<MockDraftPick>
    {
        public MockDraftPickCsvMap()
        {
            //Pick,Round,Player,School,Position,Team,ReachValue,Points,Date
            Map(m => m.pickNumber).Name("Pick");
            Map(m => m.round).Name("Round");
            Map(m => m.playerName).Name("Player");
            Map(m => m.school).Name("School");
            Map(m => m.position).Name("Position");
            Map(m => m.teamCity).Name("Team");
            Map(m => m.reachValue).Name($"ReachValue");
            Map(m => m.leagifyPoints).Name("Points");
            Map(m => m.pickDate).Name("Date");
            Map(m => m.state).Name("State");
        }
    }
}