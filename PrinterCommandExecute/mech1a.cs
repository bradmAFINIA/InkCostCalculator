using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterCommandExecute
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21", IsNullable = false)]
    public partial class Mech
    {

        private MechMechState mechStateField;

        private uint mechErrorField;

        private MechMechInput mechInputField;

        private MechMechOutput mechOutputField;

        /// <remarks/>
        public MechMechState MechState
        {
            get
            {
                return this.mechStateField;
            }
            set
            {
                this.mechStateField = value;
            }
        }

        /// <remarks/>
        public uint MechError
        {
            get
            {
                return this.mechErrorField;
            }
            set
            {
                this.mechErrorField = value;
            }
        }

        /// <remarks/>
        public MechMechInput MechInput
        {
            get
            {
                return this.mechInputField;
            }
            set
            {
                this.mechInputField = value;
            }
        }

        /// <remarks/>
        public MechMechOutput MechOutput
        {
            get
            {
                return this.mechOutputField;
            }
            set
            {
                this.mechOutputField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class MechMechState
    {

        private string mechStatusField;

        /// <remarks/>
        public string MechStatus
        {
            get
            {
                return this.mechStatusField;
            }
            set
            {
                this.mechStatusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class MechMechInput
    {

        private uint sequenceIdField;

        private uint mechInput0Field;

        private uint mechInput1Field;

        private uint mechInput2Field;

        private uint mechInput3Field;

        private uint mechInput4Field;

        /// <remarks/>
        public uint SequenceId
        {
            get
            {
                return this.sequenceIdField;
            }
            set
            {
                this.sequenceIdField = value;
            }
        }

        /// <remarks/>
        public uint MechInput0
        {
            get
            {
                return this.mechInput0Field;
            }
            set
            {
                this.mechInput0Field = value;
            }
        }

        /// <remarks/>
        public uint MechInput1
        {
            get
            {
                return this.mechInput1Field;
            }
            set
            {
                this.mechInput1Field = value;
            }
        }

        /// <remarks/>
        public uint MechInput2
        {
            get
            {
                return this.mechInput2Field;
            }
            set
            {
                this.mechInput2Field = value;
            }
        }

        /// <remarks/>
        public uint MechInput3
        {
            get
            {
                return this.mechInput3Field;
            }
            set
            {
                this.mechInput3Field = value;
            }
        }

        /// <remarks/>
        public uint MechInput4
        {
            get
            {
                return this.mechInput4Field;
            }
            set
            {
                this.mechInput4Field = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class MechMechOutput
    {

        private uint mechOutput0Field;

        private uint mechOutput1Field;

        private uint mechOutput2Field;

        private uint mechOutput3Field;

        private uint mechOutput4Field;

        /// <remarks/>
        public uint MechOutput0
        {
            get
            {
                return this.mechOutput0Field;
            }
            set
            {
                this.mechOutput0Field = value;
            }
        }

        /// <remarks/>
        public uint MechOutput1
        {
            get
            {
                return this.mechOutput1Field;
            }
            set
            {
                this.mechOutput1Field = value;
            }
        }

        /// <remarks/>
        public uint MechOutput2
        {
            get
            {
                return this.mechOutput2Field;
            }
            set
            {
                this.mechOutput2Field = value;
            }
        }

        /// <remarks/>
        public uint MechOutput3
        {
            get
            {
                return this.mechOutput3Field;
            }
            set
            {
                this.mechOutput3Field = value;
            }
        }

        /// <remarks/>
        public uint MechOutput4
        {
            get
            {
                return this.mechOutput4Field;
            }
            set
            {
                this.mechOutput4Field = value;
            }
        }
    }




}
