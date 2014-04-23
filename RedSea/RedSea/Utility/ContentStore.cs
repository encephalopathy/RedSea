using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RedSeaGame
{
    class ContentStore
    {
        // Sprites
        private static Texture2D malaria;
        private static Texture2D redBloodCell;
        private static Texture2D redBloodInfection;
        private static Texture2D whiteBloodCell;
        private static Texture2D tCell;
        private static Texture2D antibody;
        private static Texture2D malariaParticle;
        private static Texture2D attachNotice;
        private static Texture2D TCell_noalert;
        private static Texture2D TCell_alerted;

        // Sound Effects
        private static SoundEffect death;
        private static SoundEffect levelComplete;
        private static SoundEffect alarm;
        private static SoundEffect getCell1;
        private static SoundEffect getCell2;
        private static SoundEffect getCell3;
        private static SoundEffect antibodyOff;
        private static int cellGet = 0;

        public ContentStore(ContentManager content)
        {
            malaria = content.Load<Texture2D>("Cell_Textures/malaria");
            redBloodCell = content.Load<Texture2D>("Cell_Textures/red_blood_cell");
            redBloodInfection = content.Load<Texture2D>("Cell_Textures/infected_red_blood_cell");
            whiteBloodCell = content.Load<Texture2D>("Cell_Textures/white_blood_cell");
            antibody = content.Load<Texture2D>("Cell_Textures/Antibody");
            tCell = content.Load<Texture2D>("Cell_Textures/T-cell");
            attachNotice = content.Load<Texture2D>("Screen_Textures/pressSpace");
            malariaParticle = content.Load<Texture2D>("Cell_Textures/green_bubble_24");
            TCell_alerted = content.Load<Texture2D>("Cell_Textures/TCellDetectionField2");
            TCell_noalert = content.Load<Texture2D>("Cell_Textures/TCellDetectionField");

            death = content.Load<SoundEffect>("SFX/Death");
            levelComplete = content.Load<SoundEffect>("SFX/LevelComplete");
            alarm = content.Load<SoundEffect>("SFX/Alarm");
            getCell1 = content.Load<SoundEffect>("SFX/Cell1");
            getCell2 = content.Load<SoundEffect>("SFX/Cell2");
            getCell3 = content.Load<SoundEffect>("SFX/Cell3");
            antibodyOff = content.Load<SoundEffect>("SFX/AntibodyOff");
        }

        // Tectures
        public static Texture2D Malaria { get { return malaria; } }
        public static Texture2D RedBloodCell { get { return redBloodCell; } }
        public static Texture2D RedBloodInfection { get { return redBloodInfection; } }
        public static Texture2D WhiteBloodCell { get { return whiteBloodCell; } }
        public static Texture2D TCell { get { return tCell; } }
        public static Texture2D Antibody { get { return antibody; } }
        public static Texture2D MalariaParticle { get { return malariaParticle; } }
        public static Texture2D AttachNotice { get { return attachNotice; } }
        public static Texture2D TCellAlerted { get { return TCell_alerted; } }
        public static Texture2D TCellNoAlert { get { return TCell_noalert; } }

        // Sound Effects
        public static SoundEffect Death { get { return death; } }
        public static SoundEffect LevelComplete { get { return levelComplete; } }
        public static SoundEffect AntibodyOff { get { return antibodyOff; } }
        public static SoundEffect Alarm { get { return alarm; } }

        // Returns random cell sound
        public static SoundEffect GetCellSound()
        {
            switch (cellGet)
            {
                case 0:
                    cellGet = 1;
                    return getCell1;
                case 1:
                    cellGet = 2;
                    return getCell2;
                case 2:
                    cellGet = 0;
                    return getCell3;
                default:
                    return getCell1;
            }
        }
    }
}
