using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkCostCalculator
{


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11", IsNullable = false)]
    public partial class ProductUsageDyn
    {

        private Version versionField;

        private ProductUsageDynPrinterSubunit printerSubunitField;

        private ProductUsageDynConsumable[] consumableSubunitField;

        private ProductUsageDynScannerEngineSubunit scannerEngineSubunitField;

        private ProductUsageDynCopyApplicationSubunit copyApplicationSubunitField;

        private ProductUsageDynScanApplicationSubunit scanApplicationSubunitField;

        private ProductUsageDynPrintApplicationSubunit printApplicationSubunitField;

        private ProductUsageDynPECounter[] ponyExpressSubunitField;

        private ProductUsageDynMobileApplicationSubunit mobileApplicationSubunitField;

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
        public ProductUsageDynPrinterSubunit PrinterSubunit
        {
            get
            {
                return this.printerSubunitField;
            }
            set
            {
                this.printerSubunitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Consumable", IsNullable = false)]
        public ProductUsageDynConsumable[] ConsumableSubunit
        {
            get
            {
                return this.consumableSubunitField;
            }
            set
            {
                this.consumableSubunitField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynScannerEngineSubunit ScannerEngineSubunit
        {
            get
            {
                return this.scannerEngineSubunitField;
            }
            set
            {
                this.scannerEngineSubunitField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynCopyApplicationSubunit CopyApplicationSubunit
        {
            get
            {
                return this.copyApplicationSubunitField;
            }
            set
            {
                this.copyApplicationSubunitField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynScanApplicationSubunit ScanApplicationSubunit
        {
            get
            {
                return this.scanApplicationSubunitField;
            }
            set
            {
                this.scanApplicationSubunitField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynPrintApplicationSubunit PrintApplicationSubunit
        {
            get
            {
                return this.printApplicationSubunitField;
            }
            set
            {
                this.printApplicationSubunitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("PECounter", IsNullable = false)]
        public ProductUsageDynPECounter[] PonyExpressSubunit
        {
            get
            {
                return this.ponyExpressSubunitField;
            }
            set
            {
                this.ponyExpressSubunitField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynMobileApplicationSubunit MobileApplicationSubunit
        {
            get
            {
                return this.mobileApplicationSubunitField;
            }
            set
            {
                this.mobileApplicationSubunitField = value;
            }
        }
    }

    ///// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    //public partial class Version
    //{

    //    private string revisionField;

    //    private System.DateTime dateField;

    //    /// <remarks/>
    //    public string Revision
    //    {
    //        get
    //        {
    //            return this.revisionField;
    //        }
    //        set
    //        {
    //            this.revisionField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
    //    public System.DateTime Date
    //    {
    //        get
    //        {
    //            return this.dateField;
    //        }
    //        set
    //        {
    //            this.dateField = value;
    //        }
    //    }
    //}

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunit
    {

        private TotalImpressions totalImpressionsField;

        private DuplexSheets duplexSheetsField;

        private JamEvents jamEventsField;

        private uint mispickEventsField;

        private ProductUsageDynPrinterSubunitUsageByMarkingAgent usageByMarkingAgentField;

        private ProductUsageDynPrinterSubunitUsageByMedia[] usageByMediaField;

        private ProductUsageDynPrinterSubunitUsageByMediaType[] usageByMediaTypeField;

        private UsageByTimeInterval[] usageByTimeIntervalField;

        private ProductUsageDynPrinterSubunitUIButtonPressCounters uIButtonPressCountersField;

        private EWSAccessCount eWSAccessCountField;

        private NetworkImpressions networkImpressionsField;

        private WirelessNetworkImpressions wirelessNetworkImpressionsField;

        private ProductUsageDynPrinterSubunitWirelessConfigUserSelectedOption wirelessConfigUserSelectedOptionField;

        private ProductUsageDynPrinterSubunitQuietModeImpressions quietModeImpressionsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public TotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public DuplexSheets DuplexSheets
        {
            get
            {
                return this.duplexSheetsField;
            }
            set
            {
                this.duplexSheetsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public JamEvents JamEvents
        {
            get
            {
                return this.jamEventsField;
            }
            set
            {
                this.jamEventsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint MispickEvents
        {
            get
            {
                return this.mispickEventsField;
            }
            set
            {
                this.mispickEventsField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynPrinterSubunitUsageByMarkingAgent UsageByMarkingAgent
        {
            get
            {
                return this.usageByMarkingAgentField;
            }
            set
            {
                this.usageByMarkingAgentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UsageByMedia")]
        public ProductUsageDynPrinterSubunitUsageByMedia[] UsageByMedia
        {
            get
            {
                return this.usageByMediaField;
            }
            set
            {
                this.usageByMediaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UsageByMediaType")]
        public ProductUsageDynPrinterSubunitUsageByMediaType[] UsageByMediaType
        {
            get
            {
                return this.usageByMediaTypeField;
            }
            set
            {
                this.usageByMediaTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UsageByTimeInterval", Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public UsageByTimeInterval[] UsageByTimeInterval
        {
            get
            {
                return this.usageByTimeIntervalField;
            }
            set
            {
                this.usageByTimeIntervalField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynPrinterSubunitUIButtonPressCounters UIButtonPressCounters
        {
            get
            {
                return this.uIButtonPressCountersField;
            }
            set
            {
                this.uIButtonPressCountersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public EWSAccessCount EWSAccessCount
        {
            get
            {
                return this.eWSAccessCountField;
            }
            set
            {
                this.eWSAccessCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public NetworkImpressions NetworkImpressions
        {
            get
            {
                return this.networkImpressionsField;
            }
            set
            {
                this.networkImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public WirelessNetworkImpressions WirelessNetworkImpressions
        {
            get
            {
                return this.wirelessNetworkImpressionsField;
            }
            set
            {
                this.wirelessNetworkImpressionsField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynPrinterSubunitWirelessConfigUserSelectedOption WirelessConfigUserSelectedOption
        {
            get
            {
                return this.wirelessConfigUserSelectedOptionField;
            }
            set
            {
                this.wirelessConfigUserSelectedOptionField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynPrinterSubunitQuietModeImpressions QuietModeImpressions
        {
            get
            {
                return this.quietModeImpressionsField;
            }
            set
            {
                this.quietModeImpressionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class TotalImpressions
    {

        private uint pEIDField;

        private bool pEIDFieldSpecified;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PEIDSpecified
        {
            get
            {
                return this.pEIDFieldSpecified;
            }
            set
            {
                this.pEIDFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class DuplexSheets
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class JamEvents
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunitUsageByMarkingAgent
    {

        private CumulativeMarkingAgentUsed cumulativeMarkingAgentUsedField;

        private CumulativeHPMarkingAgentUsed cumulativeHPMarkingAgentUsedField;

        private CumulativeHPMarkingAgentInserted cumulativeHPMarkingAgentInsertedField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
        public CumulativeMarkingAgentUsed CumulativeMarkingAgentUsed
        {
            get
            {
                return this.cumulativeMarkingAgentUsedField;
            }
            set
            {
                this.cumulativeMarkingAgentUsedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
        public CumulativeHPMarkingAgentUsed CumulativeHPMarkingAgentUsed
        {
            get
            {
                return this.cumulativeHPMarkingAgentUsedField;
            }
            set
            {
                this.cumulativeHPMarkingAgentUsedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public CumulativeHPMarkingAgentInserted CumulativeHPMarkingAgentInserted
        {
            get
            {
                return this.cumulativeHPMarkingAgentInsertedField;
            }
            set
            {
                this.cumulativeHPMarkingAgentInsertedField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10", IsNullable = false)]
    public partial class CumulativeMarkingAgentUsed
    {

        private uint valueFloatField;

        private string unitField;

        private uint pEIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint ValueFloat
        {
            get
            {
                return this.valueFloatField;
            }
            set
            {
                this.valueFloatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string Unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10", IsNullable = false)]
    public partial class CumulativeHPMarkingAgentUsed
    {

        private uint valueFloatField;

        private string unitField;

        private uint pEIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint ValueFloat
        {
            get
            {
                return this.valueFloatField;
            }
            set
            {
                this.valueFloatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string Unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class CumulativeHPMarkingAgentInserted
    {

        private uint valueFloatField;

        private string unitField;

        private uint pEIDField;

        /// <remarks/>
        public uint ValueFloat
        {
            get
            {
                return this.valueFloatField;
            }
            set
            {
                this.valueFloatField = value;
            }
        }

        /// <remarks/>
        public string Unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunitUsageByMedia
    {

        private TotalImpressions totalImpressionsField;

        private string mediaSizeNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public TotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string MediaSizeName
        {
            get
            {
                return this.mediaSizeNameField;
            }
            set
            {
                this.mediaSizeNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunitUsageByMediaType
    {

        private TotalImpressions totalImpressionsField;

        private UsageByJobSize[] usageByJobSizeField;

        private UsageByQuality usageByQualityField;

        private string mediaTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public TotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UsageByJobSize", Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public UsageByJobSize[] UsageByJobSize
        {
            get
            {
                return this.usageByJobSizeField;
            }
            set
            {
                this.usageByJobSizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public UsageByQuality UsageByQuality
        {
            get
            {
                return this.usageByQualityField;
            }
            set
            {
                this.usageByQualityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string MediaType
        {
            get
            {
                return this.mediaTypeField;
            }
            set
            {
                this.mediaTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class UsageByJobSize
    {

        private UsageByJobSizeTotalImpressions totalImpressionsField;

        private string pagesPerJobField;

        /// <remarks/>
        public UsageByJobSizeTotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }

        /// <remarks/>
        public string PagesPerJob
        {
            get
            {
                return this.pagesPerJobField;
            }
            set
            {
                this.pagesPerJobField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByJobSizeTotalImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class UsageByQuality
    {

        private UsageByQualityNormalImpressions normalImpressionsField;

        private UsageByQualityDraftImpressions draftImpressionsField;

        private UsageByQualityBetterImpressions betterImpressionsField;

        /// <remarks/>
        public UsageByQualityNormalImpressions NormalImpressions
        {
            get
            {
                return this.normalImpressionsField;
            }
            set
            {
                this.normalImpressionsField = value;
            }
        }

        /// <remarks/>
        public UsageByQualityDraftImpressions DraftImpressions
        {
            get
            {
                return this.draftImpressionsField;
            }
            set
            {
                this.draftImpressionsField = value;
            }
        }

        /// <remarks/>
        public UsageByQualityBetterImpressions BetterImpressions
        {
            get
            {
                return this.betterImpressionsField;
            }
            set
            {
                this.betterImpressionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByQualityNormalImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByQualityDraftImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByQualityBetterImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class UsageByTimeInterval
    {

        private UsageByTimeIntervalJobCount jobCountField;

        private string jobDurationField;

        /// <remarks/>
        public UsageByTimeIntervalJobCount JobCount
        {
            get
            {
                return this.jobCountField;
            }
            set
            {
                this.jobCountField = value;
            }
        }

        /// <remarks/>
        public string JobDuration
        {
            get
            {
                return this.jobDurationField;
            }
            set
            {
                this.jobDurationField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByTimeIntervalJobCount
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunitUIButtonPressCounters
    {

        private ButtonPressCount buttonPressCountField;

        private string buttonPressOptionsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ButtonPressCount ButtonPressCount
        {
            get
            {
                return this.buttonPressCountField;
            }
            set
            {
                this.buttonPressCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ButtonPressOptions
        {
            get
            {
                return this.buttonPressOptionsField;
            }
            set
            {
                this.buttonPressOptionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class ButtonPressCount
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class EWSAccessCount
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class NetworkImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class WirelessNetworkImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunitWirelessConfigUserSelectedOption
    {

        private uint pEIDField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunitQuietModeImpressions
    {

        private ProductUsageDynPrinterSubunitQuietModeImpressionsTotalImpressions totalImpressionsField;

        /// <remarks/>
        public ProductUsageDynPrinterSubunitQuietModeImpressionsTotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrinterSubunitQuietModeImpressionsTotalImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynConsumable
    {

        private string markerColorField;

        private string consumableTypeEnumField;

        private uint cumulativeConsumableCountField;

        private CumulativeMarkingAgentUsed cumulativeMarkingAgentUsedField;

        private uint consumableRawPercentageLevelRemainingField;

        private PVPCartridgeCounter[] pVPCartridgeCounterField;

        private UsageByMarkingAgentCount[] usageByMarkingAgentCountField;

        private UsageByPenStall[] usageByPenStallField;

        private RefilledCount refilledCountField;

        private PenInsertionTriggerSnapshotCounter penInsertionTriggerSnapshotCounterField;

        private CumulativeHPMarkingAgentUsed cumulativeHPMarkingAgentUsedField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string MarkerColor
        {
            get
            {
                return this.markerColorField;
            }
            set
            {
                this.markerColorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableTypeEnum
        {
            get
            {
                return this.consumableTypeEnumField;
            }
            set
            {
                this.consumableTypeEnumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
        public uint CumulativeConsumableCount
        {
            get
            {
                return this.cumulativeConsumableCountField;
            }
            set
            {
                this.cumulativeConsumableCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
        public CumulativeMarkingAgentUsed CumulativeMarkingAgentUsed
        {
            get
            {
                return this.cumulativeMarkingAgentUsedField;
            }
            set
            {
                this.cumulativeMarkingAgentUsedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint ConsumableRawPercentageLevelRemaining
        {
            get
            {
                return this.consumableRawPercentageLevelRemainingField;
            }
            set
            {
                this.consumableRawPercentageLevelRemainingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PVPCartridgeCounter", Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public PVPCartridgeCounter[] PVPCartridgeCounter
        {
            get
            {
                return this.pVPCartridgeCounterField;
            }
            set
            {
                this.pVPCartridgeCounterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UsageByMarkingAgentCount", Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public UsageByMarkingAgentCount[] UsageByMarkingAgentCount
        {
            get
            {
                return this.usageByMarkingAgentCountField;
            }
            set
            {
                this.usageByMarkingAgentCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UsageByPenStall", Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public UsageByPenStall[] UsageByPenStall
        {
            get
            {
                return this.usageByPenStallField;
            }
            set
            {
                this.usageByPenStallField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public RefilledCount RefilledCount
        {
            get
            {
                return this.refilledCountField;
            }
            set
            {
                this.refilledCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public PenInsertionTriggerSnapshotCounter PenInsertionTriggerSnapshotCounter
        {
            get
            {
                return this.penInsertionTriggerSnapshotCounterField;
            }
            set
            {
                this.penInsertionTriggerSnapshotCounterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
        public CumulativeHPMarkingAgentUsed CumulativeHPMarkingAgentUsed
        {
            get
            {
                return this.cumulativeHPMarkingAgentUsedField;
            }
            set
            {
                this.cumulativeHPMarkingAgentUsedField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class PVPCartridgeCounter
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class UsageByMarkingAgentCount
    {

        private UsageByMarkingAgentCountMarkingAgentCount markingAgentCountField;

        private string markingAgentCountTypeField;

        /// <remarks/>
        public UsageByMarkingAgentCountMarkingAgentCount MarkingAgentCount
        {
            get
            {
                return this.markingAgentCountField;
            }
            set
            {
                this.markingAgentCountField = value;
            }
        }

        /// <remarks/>
        public string MarkingAgentCountType
        {
            get
            {
                return this.markingAgentCountTypeField;
            }
            set
            {
                this.markingAgentCountTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByMarkingAgentCountMarkingAgentCount
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class UsageByPenStall
    {

        private UsageByPenStallPenStall[] penStallField;

        private UsageByPenStallTijGenAndVersion tijGenAndVersionField;

        private UsageByPenStallNonHPFlagCounter nonHPFlagCounterField;

        private uint penStallNumberField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PenStall")]
        public UsageByPenStallPenStall[] PenStall
        {
            get
            {
                return this.penStallField;
            }
            set
            {
                this.penStallField = value;
            }
        }

        /// <remarks/>
        public UsageByPenStallTijGenAndVersion TijGenAndVersion
        {
            get
            {
                return this.tijGenAndVersionField;
            }
            set
            {
                this.tijGenAndVersionField = value;
            }
        }

        /// <remarks/>
        public UsageByPenStallNonHPFlagCounter NonHPFlagCounter
        {
            get
            {
                return this.nonHPFlagCounterField;
            }
            set
            {
                this.nonHPFlagCounterField = value;
            }
        }

        /// <remarks/>
        public uint PenStallNumber
        {
            get
            {
                return this.penStallNumberField;
            }
            set
            {
                this.penStallNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByPenStallPenStall
    {

        private UsageByPenStallPenStallPenStallValue penStallValueField;

        private string penStallValueLocationField;

        /// <remarks/>
        public UsageByPenStallPenStallPenStallValue PenStallValue
        {
            get
            {
                return this.penStallValueField;
            }
            set
            {
                this.penStallValueField = value;
            }
        }

        /// <remarks/>
        public string PenStallValueLocation
        {
            get
            {
                return this.penStallValueLocationField;
            }
            set
            {
                this.penStallValueLocationField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByPenStallPenStallPenStallValue
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByPenStallTijGenAndVersion
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class UsageByPenStallNonHPFlagCounter
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class RefilledCount
    {

        private RefilledCountCounterfeitRefilledCount counterfeitRefilledCountField;

        private uint genuineRefilledCountField;

        /// <remarks/>
        public RefilledCountCounterfeitRefilledCount CounterfeitRefilledCount
        {
            get
            {
                return this.counterfeitRefilledCountField;
            }
            set
            {
                this.counterfeitRefilledCountField = value;
            }
        }

        /// <remarks/>
        public uint GenuineRefilledCount
        {
            get
            {
                return this.genuineRefilledCountField;
            }
            set
            {
                this.genuineRefilledCountField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class RefilledCountCounterfeitRefilledCount
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class PenInsertionTriggerSnapshotCounter
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynScannerEngineSubunit
    {

        private ScanImages scanImagesField;

        private ProductUsageDynScannerEngineSubunitUsageByScanMediaSize[] usageByScanMediaSizeField;

        private uint flatbedImagesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ScanImages ScanImages
        {
            get
            {
                return this.scanImagesField;
            }
            set
            {
                this.scanImagesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UsageByScanMediaSize")]
        public ProductUsageDynScannerEngineSubunitUsageByScanMediaSize[] UsageByScanMediaSize
        {
            get
            {
                return this.usageByScanMediaSizeField;
            }
            set
            {
                this.usageByScanMediaSizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint FlatbedImages
        {
            get
            {
                return this.flatbedImagesField;
            }
            set
            {
                this.flatbedImagesField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class ScanImages
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynScannerEngineSubunitUsageByScanMediaSize
    {

        private ScanImages scanImagesField;

        private string scanMediaSizeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ScanImages ScanImages
        {
            get
            {
                return this.scanImagesField;
            }
            set
            {
                this.scanImagesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ScanMediaSize
        {
            get
            {
                return this.scanMediaSizeField;
            }
            set
            {
                this.scanMediaSizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynCopyApplicationSubunit
    {

        private TotalImpressions totalImpressionsField;

        private uint colorImpressionsField;

        private uint monochromeImpressionsField;

        private uint adfImagesField;

        private uint flatbedImagesField;

        private ProductUsageDynCopyApplicationSubunitUsageByMedia usageByMediaField;

        private ProductUsageDynCopyApplicationSubunitTotalAdfLegallmages totalAdfLegallmagesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public TotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint ColorImpressions
        {
            get
            {
                return this.colorImpressionsField;
            }
            set
            {
                this.colorImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint MonochromeImpressions
        {
            get
            {
                return this.monochromeImpressionsField;
            }
            set
            {
                this.monochromeImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint AdfImages
        {
            get
            {
                return this.adfImagesField;
            }
            set
            {
                this.adfImagesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint FlatbedImages
        {
            get
            {
                return this.flatbedImagesField;
            }
            set
            {
                this.flatbedImagesField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynCopyApplicationSubunitUsageByMedia UsageByMedia
        {
            get
            {
                return this.usageByMediaField;
            }
            set
            {
                this.usageByMediaField = value;
            }
        }

        /// <remarks/>
        public ProductUsageDynCopyApplicationSubunitTotalAdfLegallmages TotalAdfLegallmages
        {
            get
            {
                return this.totalAdfLegallmagesField;
            }
            set
            {
                this.totalAdfLegallmagesField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynCopyApplicationSubunitUsageByMedia
    {

        private TotalImpressions totalImpressionsField;

        private string mediaSizeNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public TotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string MediaSizeName
        {
            get
            {
                return this.mediaSizeNameField;
            }
            set
            {
                this.mediaSizeNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynCopyApplicationSubunitTotalAdfLegallmages
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynScanApplicationSubunit
    {

        private uint adfImagesField;

        private uint flatbedImagesField;

        private ScanToHostImages scanToHostImagesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint AdfImages
        {
            get
            {
                return this.adfImagesField;
            }
            set
            {
                this.adfImagesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public uint FlatbedImages
        {
            get
            {
                return this.flatbedImagesField;
            }
            set
            {
                this.flatbedImagesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ScanToHostImages ScanToHostImages
        {
            get
            {
                return this.scanToHostImagesField;
            }
            set
            {
                this.scanToHostImagesField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class ScanToHostImages
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPrintApplicationSubunit
    {

        private TotalImpressions totalImpressionsField;

        private PhotoImpressions photoImpressionsField;

        private MediaSize_4x6in_10x15cm_Impressions mediaSize_4x6in_10x15cm_ImpressionsField;

        private CloudPrintImpressions cloudPrintImpressionsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public TotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public PhotoImpressions PhotoImpressions
        {
            get
            {
                return this.photoImpressionsField;
            }
            set
            {
                this.photoImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public MediaSize_4x6in_10x15cm_Impressions MediaSize_4x6in_10x15cm_Impressions
        {
            get
            {
                return this.mediaSize_4x6in_10x15cm_ImpressionsField;
            }
            set
            {
                this.mediaSize_4x6in_10x15cm_ImpressionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public CloudPrintImpressions CloudPrintImpressions
        {
            get
            {
                return this.cloudPrintImpressionsField;
            }
            set
            {
                this.cloudPrintImpressionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class PhotoImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class MediaSize_4x6in_10x15cm_Impressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class CloudPrintImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynPECounter
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunit
    {

        private ProductUsageDynMobileApplicationSubunitTotalUsage[] totalUsageField;

        private ProductUsageDynMobileApplicationSubunitDocumentUsage[] documentUsageField;

        private ProductUsageDynMobileApplicationSubunitPhotoUsage[] photoUsageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TotalUsage")]
        public ProductUsageDynMobileApplicationSubunitTotalUsage[] TotalUsage
        {
            get
            {
                return this.totalUsageField;
            }
            set
            {
                this.totalUsageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DocumentUsage")]
        public ProductUsageDynMobileApplicationSubunitDocumentUsage[] DocumentUsage
        {
            get
            {
                return this.documentUsageField;
            }
            set
            {
                this.documentUsageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PhotoUsage")]
        public ProductUsageDynMobileApplicationSubunitPhotoUsage[] PhotoUsage
        {
            get
            {
                return this.photoUsageField;
            }
            set
            {
                this.photoUsageField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitTotalUsage
    {

        private ProductUsageDynMobileApplicationSubunitTotalUsageCounter counterField;

        private string remoteDeviceTypeField;

        private string driverTypeField;

        /// <remarks/>
        public ProductUsageDynMobileApplicationSubunitTotalUsageCounter Counter
        {
            get
            {
                return this.counterField;
            }
            set
            {
                this.counterField = value;
            }
        }

        /// <remarks/>
        public string RemoteDeviceType
        {
            get
            {
                return this.remoteDeviceTypeField;
            }
            set
            {
                this.remoteDeviceTypeField = value;
            }
        }

        /// <remarks/>
        public string DriverType
        {
            get
            {
                return this.driverTypeField;
            }
            set
            {
                this.driverTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitTotalUsageCounter
    {

        private ProductUsageDynMobileApplicationSubunitTotalUsageCounterTotalImpressions totalImpressionsField;

        /// <remarks/>
        public ProductUsageDynMobileApplicationSubunitTotalUsageCounterTotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitTotalUsageCounterTotalImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitDocumentUsage
    {

        private ProductUsageDynMobileApplicationSubunitDocumentUsageCounter counterField;

        private string remoteDeviceTypeField;

        /// <remarks/>
        public ProductUsageDynMobileApplicationSubunitDocumentUsageCounter Counter
        {
            get
            {
                return this.counterField;
            }
            set
            {
                this.counterField = value;
            }
        }

        /// <remarks/>
        public string RemoteDeviceType
        {
            get
            {
                return this.remoteDeviceTypeField;
            }
            set
            {
                this.remoteDeviceTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitDocumentUsageCounter
    {

        private ProductUsageDynMobileApplicationSubunitDocumentUsageCounterTotalImpressions totalImpressionsField;

        /// <remarks/>
        public ProductUsageDynMobileApplicationSubunitDocumentUsageCounterTotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitDocumentUsageCounterTotalImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitPhotoUsage
    {

        private ProductUsageDynMobileApplicationSubunitPhotoUsageCounter counterField;

        private string remoteDeviceTypeField;

        /// <remarks/>
        public ProductUsageDynMobileApplicationSubunitPhotoUsageCounter Counter
        {
            get
            {
                return this.counterField;
            }
            set
            {
                this.counterField = value;
            }
        }

        /// <remarks/>
        public string RemoteDeviceType
        {
            get
            {
                return this.remoteDeviceTypeField;
            }
            set
            {
                this.remoteDeviceTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitPhotoUsageCounter
    {

        private ProductUsageDynMobileApplicationSubunitPhotoUsageCounterTotalImpressions totalImpressionsField;

        /// <remarks/>
        public ProductUsageDynMobileApplicationSubunitPhotoUsageCounterTotalImpressions TotalImpressions
        {
            get
            {
                return this.totalImpressionsField;
            }
            set
            {
                this.totalImpressionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/productusagedyn/2007/12/11")]
    public partial class ProductUsageDynMobileApplicationSubunitPhotoUsageCounterTotalImpressions
    {

        private uint pEIDField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint PEID
        {
            get
            {
                return this.pEIDField;
            }
            set
            {
                this.pEIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }


}
