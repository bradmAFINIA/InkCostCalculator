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
    public partial class OemsiMediapathDyn
    {

        private Version versionField;

        private OemsiMediapathDynOemsiMediapathInfo oemsiMediapathInfoField;

        private OemsiMediapathDynOemsiMediapathSettings oemsiMediapathSettingsField;

        private OemsiMediapathDynMech mechField;

        private OemsiMediapathDynBridge bridgeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public Version Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        public OemsiMediapathDynOemsiMediapathInfo OemsiMediapathInfo
        {
            get
            {
                return this.oemsiMediapathInfoField;
            }
            set
            {
                this.oemsiMediapathInfoField = value;
            }
        }

        /// <remarks/>
        public OemsiMediapathDynOemsiMediapathSettings OemsiMediapathSettings
        {
            get
            {
                return this.oemsiMediapathSettingsField;
            }
            set
            {
                this.oemsiMediapathSettingsField = value;
            }
        }

        /// <remarks/>
        public OemsiMediapathDynMech Mech
        {
            get
            {
                return this.mechField;
            }
            set
            {
                this.mechField = value;
            }
        }

        /// <remarks/>
        public OemsiMediapathDynBridge Bridge
        {
            get
            {
                return this.bridgeField;
            }
            set
            {
                this.bridgeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class Version
    {

        private string revisionField;

        private System.DateTime dateField;

        /// <remarks/>
        public string Revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynOemsiMediapathInfo
    {

        private OemsiMediapathDynOemsiMediapathInfoBaseFirmwareVersion baseFirmwareVersionField;

        private OemsiMediapathDynOemsiMediapathInfoOemMediapathFirmwareVersion oemMediapathFirmwareVersionField;

        private uint oemRo0Field;

        private uint oemRo1Field;

        private uint oemRo2Field;

        private uint oemRo3Field;

        private uint oemRo4Field;

        private uint oemRo5Field;

        private uint oemRo6Field;

        private uint oemRo7Field;

        private uint oemRo8Field;

        private uint oemRo9Field;

        private uint oemRo10Field;

        private uint oemRo11Field;

        private uint oemRo12Field;

        private uint oemRo13Field;

        private uint oemRo14Field;

        private uint oemRo15Field;

        private uint oemRo16Field;

        private uint oemRo17Field;

        private uint oemRo18Field;

        private uint oemRo19Field;

        private uint oemRo20Field;

        private uint oemRo21Field;

        private uint oemRo22Field;

        private uint oemRo23Field;

        private uint oemRo24Field;

        private uint oemRo25Field;

        private uint oemRo26Field;

        private uint oemRo27Field;

        private uint oemRo28Field;

        private uint oemRo29Field;

        private uint oemRo30Field;

        private uint oemRo31Field;

        private OemsiMediapathDynOemsiMediapathInfoConsumableSerialNumber consumableSerialNumberField;

        /// <remarks/>
        public OemsiMediapathDynOemsiMediapathInfoBaseFirmwareVersion BaseFirmwareVersion
        {
            get
            {
                return this.baseFirmwareVersionField;
            }
            set
            {
                this.baseFirmwareVersionField = value;
            }
        }

        /// <remarks/>
        public OemsiMediapathDynOemsiMediapathInfoOemMediapathFirmwareVersion OemMediapathFirmwareVersion
        {
            get
            {
                return this.oemMediapathFirmwareVersionField;
            }
            set
            {
                this.oemMediapathFirmwareVersionField = value;
            }
        }

        /// <remarks/>
        public uint OemRo0
        {
            get
            {
                return this.oemRo0Field;
            }
            set
            {
                this.oemRo0Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo1
        {
            get
            {
                return this.oemRo1Field;
            }
            set
            {
                this.oemRo1Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo2
        {
            get
            {
                return this.oemRo2Field;
            }
            set
            {
                this.oemRo2Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo3
        {
            get
            {
                return this.oemRo3Field;
            }
            set
            {
                this.oemRo3Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo4
        {
            get
            {
                return this.oemRo4Field;
            }
            set
            {
                this.oemRo4Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo5
        {
            get
            {
                return this.oemRo5Field;
            }
            set
            {
                this.oemRo5Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo6
        {
            get
            {
                return this.oemRo6Field;
            }
            set
            {
                this.oemRo6Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo7
        {
            get
            {
                return this.oemRo7Field;
            }
            set
            {
                this.oemRo7Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo8
        {
            get
            {
                return this.oemRo8Field;
            }
            set
            {
                this.oemRo8Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo9
        {
            get
            {
                return this.oemRo9Field;
            }
            set
            {
                this.oemRo9Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo10
        {
            get
            {
                return this.oemRo10Field;
            }
            set
            {
                this.oemRo10Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo11
        {
            get
            {
                return this.oemRo11Field;
            }
            set
            {
                this.oemRo11Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo12
        {
            get
            {
                return this.oemRo12Field;
            }
            set
            {
                this.oemRo12Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo13
        {
            get
            {
                return this.oemRo13Field;
            }
            set
            {
                this.oemRo13Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo14
        {
            get
            {
                return this.oemRo14Field;
            }
            set
            {
                this.oemRo14Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo15
        {
            get
            {
                return this.oemRo15Field;
            }
            set
            {
                this.oemRo15Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo16
        {
            get
            {
                return this.oemRo16Field;
            }
            set
            {
                this.oemRo16Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo17
        {
            get
            {
                return this.oemRo17Field;
            }
            set
            {
                this.oemRo17Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo18
        {
            get
            {
                return this.oemRo18Field;
            }
            set
            {
                this.oemRo18Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo19
        {
            get
            {
                return this.oemRo19Field;
            }
            set
            {
                this.oemRo19Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo20
        {
            get
            {
                return this.oemRo20Field;
            }
            set
            {
                this.oemRo20Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo21
        {
            get
            {
                return this.oemRo21Field;
            }
            set
            {
                this.oemRo21Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo22
        {
            get
            {
                return this.oemRo22Field;
            }
            set
            {
                this.oemRo22Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo23
        {
            get
            {
                return this.oemRo23Field;
            }
            set
            {
                this.oemRo23Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo24
        {
            get
            {
                return this.oemRo24Field;
            }
            set
            {
                this.oemRo24Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo25
        {
            get
            {
                return this.oemRo25Field;
            }
            set
            {
                this.oemRo25Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo26
        {
            get
            {
                return this.oemRo26Field;
            }
            set
            {
                this.oemRo26Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo27
        {
            get
            {
                return this.oemRo27Field;
            }
            set
            {
                this.oemRo27Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo28
        {
            get
            {
                return this.oemRo28Field;
            }
            set
            {
                this.oemRo28Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo29
        {
            get
            {
                return this.oemRo29Field;
            }
            set
            {
                this.oemRo29Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo30
        {
            get
            {
                return this.oemRo30Field;
            }
            set
            {
                this.oemRo30Field = value;
            }
        }

        /// <remarks/>
        public uint OemRo31
        {
            get
            {
                return this.oemRo31Field;
            }
            set
            {
                this.oemRo31Field = value;
            }
        }

        /// <remarks/>
        public OemsiMediapathDynOemsiMediapathInfoConsumableSerialNumber ConsumableSerialNumber
        {
            get
            {
                return this.consumableSerialNumberField;
            }
            set
            {
                this.consumableSerialNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynOemsiMediapathInfoBaseFirmwareVersion
    {

        private string revisionField;

        private System.DateTime dateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string Revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", DataType = "date")]
        public System.DateTime Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynOemsiMediapathInfoOemMediapathFirmwareVersion
    {

        private string revisionField;

        private System.DateTime dateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string Revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", DataType = "date")]
        public System.DateTime Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynOemsiMediapathInfoConsumableSerialNumber
    {

        private string supplyCyanField;

        private string supplyMagentaField;

        private string supplyYellowField;

        private string supplyBlackField;

        /// <remarks/>
        public string SupplyCyan
        {
            get
            {
                return this.supplyCyanField;
            }
            set
            {
                this.supplyCyanField = value;
            }
        }

        /// <remarks/>
        public string SupplyMagenta
        {
            get
            {
                return this.supplyMagentaField;
            }
            set
            {
                this.supplyMagentaField = value;
            }
        }

        /// <remarks/>
        public string SupplyYellow
        {
            get
            {
                return this.supplyYellowField;
            }
            set
            {
                this.supplyYellowField = value;
            }
        }

        /// <remarks/>
        public string SupplyBlack
        {
            get
            {
                return this.supplyBlackField;
            }
            set
            {
                this.supplyBlackField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynOemsiMediapathSettings
    {

        private uint oemRw0Field;

        private uint oemRw1Field;

        private uint oemRw2Field;

        private uint oemRw3Field;

        private uint oemRw4Field;

        private uint oemRw5Field;

        private uint oemRw6Field;

        private uint oemRw7Field;

        private uint oemRw8Field;

        private uint oemRw9Field;

        private uint oemRw10Field;

        private uint oemRw11Field;

        private uint oemRw12Field;

        private uint oemRw13Field;

        private uint oemRw14Field;

        private uint oemRw15Field;

        private OemsiMediapathDynOemsiMediapathSettingsMediaTrayPositionAdjustment mediaTrayPositionAdjustmentField;

        private uint oemNvmUserResettableField;

        private uint oemNvmPartialResettableField;

        private uint oemNvmSemiResettableField;

        private uint oemNvmFullResettableField;

        private uint oemNvm0Field;

        private uint oemNvm1Field;

        private uint oemNvm2Field;

        private uint oemNvm3Field;

        private uint oemNvm4Field;

        private uint oemNvm5Field;

        private uint oemNvm6Field;

        private uint oemNvm7Field;

        private uint oemNvm8Field;

        private uint oemNvm9Field;

        private uint oemNvm10Field;

        private uint oemNvm11Field;

        private uint oemNvm12Field;

        private uint oemNvm13Field;

        private uint oemNvm14Field;

        private uint oemNvm15Field;

//        private OemsiMediapathDynOemsiMediapathSettingsScratchpadSettings scratchpadSettingsField;

        /// <remarks/>
        public uint OemRw0
        {
            get
            {
                return this.oemRw0Field;
            }
            set
            {
                this.oemRw0Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw1
        {
            get
            {
                return this.oemRw1Field;
            }
            set
            {
                this.oemRw1Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw2
        {
            get
            {
                return this.oemRw2Field;
            }
            set
            {
                this.oemRw2Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw3
        {
            get
            {
                return this.oemRw3Field;
            }
            set
            {
                this.oemRw3Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw4
        {
            get
            {
                return this.oemRw4Field;
            }
            set
            {
                this.oemRw4Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw5
        {
            get
            {
                return this.oemRw5Field;
            }
            set
            {
                this.oemRw5Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw6
        {
            get
            {
                return this.oemRw6Field;
            }
            set
            {
                this.oemRw6Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw7
        {
            get
            {
                return this.oemRw7Field;
            }
            set
            {
                this.oemRw7Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw8
        {
            get
            {
                return this.oemRw8Field;
            }
            set
            {
                this.oemRw8Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw9
        {
            get
            {
                return this.oemRw9Field;
            }
            set
            {
                this.oemRw9Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw10
        {
            get
            {
                return this.oemRw10Field;
            }
            set
            {
                this.oemRw10Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw11
        {
            get
            {
                return this.oemRw11Field;
            }
            set
            {
                this.oemRw11Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw12
        {
            get
            {
                return this.oemRw12Field;
            }
            set
            {
                this.oemRw12Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw13
        {
            get
            {
                return this.oemRw13Field;
            }
            set
            {
                this.oemRw13Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw14
        {
            get
            {
                return this.oemRw14Field;
            }
            set
            {
                this.oemRw14Field = value;
            }
        }

        /// <remarks/>
        public uint OemRw15
        {
            get
            {
                return this.oemRw15Field;
            }
            set
            {
                this.oemRw15Field = value;
            }
        }

        /// <remarks/>
        public OemsiMediapathDynOemsiMediapathSettingsMediaTrayPositionAdjustment MediaTrayPositionAdjustment
        {
            get
            {
                return this.mediaTrayPositionAdjustmentField;
            }
            set
            {
                this.mediaTrayPositionAdjustmentField = value;
            }
        }

        /// <remarks/>
        public uint OemNvmUserResettable
        {
            get
            {
                return this.oemNvmUserResettableField;
            }
            set
            {
                this.oemNvmUserResettableField = value;
            }
        }

        /// <remarks/>
        public uint OemNvmPartialResettable
        {
            get
            {
                return this.oemNvmPartialResettableField;
            }
            set
            {
                this.oemNvmPartialResettableField = value;
            }
        }

        /// <remarks/>
        public uint OemNvmSemiResettable
        {
            get
            {
                return this.oemNvmSemiResettableField;
            }
            set
            {
                this.oemNvmSemiResettableField = value;
            }
        }

        /// <remarks/>
        public uint OemNvmFullResettable
        {
            get
            {
                return this.oemNvmFullResettableField;
            }
            set
            {
                this.oemNvmFullResettableField = value;
            }
        }

        /// <remarks/>
        public uint OemNvm0
        {
            get
            {
                return this.oemNvm0Field;
            }
            set
            {
                this.oemNvm0Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm1
        {
            get
            {
                return this.oemNvm1Field;
            }
            set
            {
                this.oemNvm1Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm2
        {
            get
            {
                return this.oemNvm2Field;
            }
            set
            {
                this.oemNvm2Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm3
        {
            get
            {
                return this.oemNvm3Field;
            }
            set
            {
                this.oemNvm3Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm4
        {
            get
            {
                return this.oemNvm4Field;
            }
            set
            {
                this.oemNvm4Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm5
        {
            get
            {
                return this.oemNvm5Field;
            }
            set
            {
                this.oemNvm5Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm6
        {
            get
            {
                return this.oemNvm6Field;
            }
            set
            {
                this.oemNvm6Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm7
        {
            get
            {
                return this.oemNvm7Field;
            }
            set
            {
                this.oemNvm7Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm8
        {
            get
            {
                return this.oemNvm8Field;
            }
            set
            {
                this.oemNvm8Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm9
        {
            get
            {
                return this.oemNvm9Field;
            }
            set
            {
                this.oemNvm9Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm10
        {
            get
            {
                return this.oemNvm10Field;
            }
            set
            {
                this.oemNvm10Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm11
        {
            get
            {
                return this.oemNvm11Field;
            }
            set
            {
                this.oemNvm11Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm12
        {
            get
            {
                return this.oemNvm12Field;
            }
            set
            {
                this.oemNvm12Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm13
        {
            get
            {
                return this.oemNvm13Field;
            }
            set
            {
                this.oemNvm13Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm14
        {
            get
            {
                return this.oemNvm14Field;
            }
            set
            {
                this.oemNvm14Field = value;
            }
        }

        /// <remarks/>
        public uint OemNvm15
        {
            get
            {
                return this.oemNvm15Field;
            }
            set
            {
                this.oemNvm15Field = value;
            }
        }

        /// <remarks/>
        //public OemsiMediapathDynOemsiMediapathSettingsScratchpadSettings ScratchpadSettings
        //{
        //    get
        //    {
        //        return this.scratchpadSettingsField;
        //    }
        //    set
        //    {
        //        this.scratchpadSettingsField = value;
        //    }
        //}
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynOemsiMediapathSettingsMediaTrayPositionAdjustment
    {

        private decimal acrossMediapathField;

        private decimal alongMediapathField;

        private string mediapathUnitField;

        /// <remarks/>
        public decimal AcrossMediapath
        {
            get
            {
                return this.acrossMediapathField;
            }
            set
            {
                this.acrossMediapathField = value;
            }
        }

        /// <remarks/>
        public decimal AlongMediapath
        {
            get
            {
                return this.alongMediapathField;
            }
            set
            {
                this.alongMediapathField = value;
            }
        }

        /// <remarks/>
        public string MediapathUnit
        {
            get
            {
                return this.mediapathUnitField;
            }
            set
            {
                this.mediapathUnitField = value;
            }
        }
    }

    ///// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    //public partial class OemsiMediapathDynOemsiMediapathSettingsScratchpadSettings
    //{

    //    private uint cyanField;

    //    private uint magentaField;

    //    private uint yellowField;

    //    private uint blackField;

    //    /// <remarks/>
    //    public uint Cyan
    //    {
    //        get
    //        {
    //            return this.cyanField;
    //        }
    //        set
    //        {
    //            this.cyanField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public uint Magenta
    //    {
    //        get
    //        {
    //            return this.magentaField;
    //        }
    //        set
    //        {
    //            this.magentaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public uint Yellow
    //    {
    //        get
    //        {
    //            return this.yellowField;
    //        }
    //        set
    //        {
    //            this.yellowField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public uint Black
    //    {
    //        get
    //        {
    //            return this.blackField;
    //        }
    //        set
    //        {
    //            this.blackField = value;
    //        }
    //    }
    //}

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynMech
    {

        private OemsiMediapathDynMechMechState mechStateField;

        /// <remarks/>
        public OemsiMediapathDynMechMechState MechState
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynMechMechState
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
    public partial class OemsiMediapathDynBridge
    {

        private OemsiMediapathDynBridgeBridgeState bridgeStateField;

        /// <remarks/>
        public OemsiMediapathDynBridgeBridgeState BridgeState
        {
            get
            {
                return this.bridgeStateField;
            }
            set
            {
                this.bridgeStateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/oemsimediapathdyn/2008/03/21")]
    public partial class OemsiMediapathDynBridgeBridgeState
    {

        private string bridgeStatusField;

        /// <remarks/>
        public string BridgeStatus
        {
            get
            {
                return this.bridgeStatusField;
            }
            set
            {
                this.bridgeStatusField = value;
            }
        }
    }
}
