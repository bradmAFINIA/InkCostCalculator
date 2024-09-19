using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkCostCalculator
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/consumableconfigdyn/2007/11/19")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/ledm/consumableconfigdyn/2007/11/19", IsNullable = false)]
    public partial class ConsumableConfigDyn
    {

        private Version versionField;

        private ConsumableConfigDynProductConsumableInfo productConsumableInfoField;

        private ConsumableConfigDynConsumableInfo[] consumableInfoField;

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
        public ConsumableConfigDynProductConsumableInfo ProductConsumableInfo
        {
            get
            {
                return this.productConsumableInfoField;
            }
            set
            {
                this.productConsumableInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ConsumableInfo")]
        public ConsumableConfigDynConsumableInfo[] ConsumableInfo
        {
            get
            {
                return this.consumableInfoField;
            }
            set
            {
                this.consumableInfoField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/consumableconfigdyn/2007/11/19")]
    public partial class ConsumableConfigDynProductConsumableInfo
    {

        private byte numOfUserReplaceableConsumablesField;

        private byte numOfNonUserReplaceableConsumablesField;

        private string alignmentModeField;

        private string cartridgeChipInfoField;

        private string consumableSlotDirectionField;

        private ushort ikField;

        private string singleCartridgeModeField;

        private string storeUsageDataField;

        private string antiTheftModeField;

        private ConsumableConfigDynProductConsumableInfoRewardsRegistrationStatus rewardsRegistrationStatusField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public byte NumOfUserReplaceableConsumables
        {
            get
            {
                return this.numOfUserReplaceableConsumablesField;
            }
            set
            {
                this.numOfUserReplaceableConsumablesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public byte NumOfNonUserReplaceableConsumables
        {
            get
            {
                return this.numOfNonUserReplaceableConsumablesField;
            }
            set
            {
                this.numOfNonUserReplaceableConsumablesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string AlignmentMode
        {
            get
            {
                return this.alignmentModeField;
            }
            set
            {
                this.alignmentModeField = value;
            }
        }

        /// <remarks/>
        public string CartridgeChipInfo
        {
            get
            {
                return this.cartridgeChipInfoField;
            }
            set
            {
                this.cartridgeChipInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableSlotDirection
        {
            get
            {
                return this.consumableSlotDirectionField;
            }
            set
            {
                this.consumableSlotDirectionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ushort IK
        {
            get
            {
                return this.ikField;
            }
            set
            {
                this.ikField = value;
            }
        }

        /// <remarks/>
        public string SingleCartridgeMode
        {
            get
            {
                return this.singleCartridgeModeField;
            }
            set
            {
                this.singleCartridgeModeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string StoreUsageData
        {
            get
            {
                return this.storeUsageDataField;
            }
            set
            {
                this.storeUsageDataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string AntiTheftMode
        {
            get
            {
                return this.antiTheftModeField;
            }
            set
            {
                this.antiTheftModeField = value;
            }
        }

        /// <remarks/>
        public ConsumableConfigDynProductConsumableInfoRewardsRegistrationStatus RewardsRegistrationStatus
        {
            get
            {
                return this.rewardsRegistrationStatusField;
            }
            set
            {
                this.rewardsRegistrationStatusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/consumableconfigdyn/2007/11/19")]
    public partial class ConsumableConfigDynProductConsumableInfoRewardsRegistrationStatus
    {

        private bool optedInField;

        private bool autoSendDataField;

        private bool promptAutoSendDataField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public bool OptedIn
        {
            get
            {
                return this.optedInField;
            }
            set
            {
                this.optedInField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public bool AutoSendData
        {
            get
            {
                return this.autoSendDataField;
            }
            set
            {
                this.autoSendDataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public bool PromptAutoSendData
        {
            get
            {
                return this.promptAutoSendDataField;
            }
            set
            {
                this.promptAutoSendDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/ledm/consumableconfigdyn/2007/11/19")]
    public partial class ConsumableConfigDynConsumableInfo
    {

        private Capacity capacityField;

        private string consumableSubBrandField;

        private string consumableSegmentClassField;

        private string consumableAcceptibilityUpgradabilityField;

        private string consumableContentTypeField;

        private string consumableFamilyNameField;

        private string consumableIDField;

        private string consumableKeyingDescriptorField;

        private string consumableLabelCodeField;

        private ConsumableLifeState consumableLifeStateField;

        private ConsumableStateAvailableActions consumableStateAvailableActionsField;

        private string consumableLevelMessagingStyleField;

        private string consumableLowUseAlgorithmField;

        private byte consumablePercentageLevelRemainingField;

        private string consumableReplaceabilityTypeField;

        private byte consumableStationField;

        private string consumableTypeEnumField;

        private RegionalCartridge regionalCartridgeField;

        private ConsumableIcon consumableIconField;

        private string consumableUniqueIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public Capacity Capacity
        {
            get
            {
                return this.capacityField;
            }
            set
            {
                this.capacityField = value;
            }
        }

        /// <remarks/>
        public string ConsumableSubBrand
        {
            get
            {
                return this.consumableSubBrandField;
            }
            set
            {
                this.consumableSubBrandField = value;
            }
        }

        /// <remarks/>
        public string ConsumableSegmentClass
        {
            get
            {
                return this.consumableSegmentClassField;
            }
            set
            {
                this.consumableSegmentClassField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableAcceptibilityUpgradability
        {
            get
            {
                return this.consumableAcceptibilityUpgradabilityField;
            }
            set
            {
                this.consumableAcceptibilityUpgradabilityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableContentType
        {
            get
            {
                return this.consumableContentTypeField;
            }
            set
            {
                this.consumableContentTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableFamilyName
        {
            get
            {
                return this.consumableFamilyNameField;
            }
            set
            {
                this.consumableFamilyNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/2008/10/10")]
        public string ConsumableID
        {
            get
            {
                return this.consumableIDField;
            }
            set
            {
                this.consumableIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableKeyingDescriptor
        {
            get
            {
                return this.consumableKeyingDescriptorField;
            }
            set
            {
                this.consumableKeyingDescriptorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableLabelCode
        {
            get
            {
                return this.consumableLabelCodeField;
            }
            set
            {
                this.consumableLabelCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ConsumableLifeState ConsumableLifeState
        {
            get
            {
                return this.consumableLifeStateField;
            }
            set
            {
                this.consumableLifeStateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ConsumableStateAvailableActions ConsumableStateAvailableActions
        {
            get
            {
                return this.consumableStateAvailableActionsField;
            }
            set
            {
                this.consumableStateAvailableActionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableLevelMessagingStyle
        {
            get
            {
                return this.consumableLevelMessagingStyleField;
            }
            set
            {
                this.consumableLevelMessagingStyleField = value;
            }
        }

        /// <remarks/>
        public string ConsumableLowUseAlgorithm
        {
            get
            {
                return this.consumableLowUseAlgorithmField;
            }
            set
            {
                this.consumableLowUseAlgorithmField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public byte ConsumablePercentageLevelRemaining
        {
            get
            {
                return this.consumablePercentageLevelRemainingField;
            }
            set
            {
                this.consumablePercentageLevelRemainingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public string ConsumableReplaceabilityType
        {
            get
            {
                return this.consumableReplaceabilityTypeField;
            }
            set
            {
                this.consumableReplaceabilityTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public byte ConsumableStation
        {
            get
            {
                return this.consumableStationField;
            }
            set
            {
                this.consumableStationField = value;
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public RegionalCartridge RegionalCartridge
        {
            get
            {
                return this.regionalCartridgeField;
            }
            set
            {
                this.regionalCartridgeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
        public ConsumableIcon ConsumableIcon
        {
            get
            {
                return this.consumableIconField;
            }
            set
            {
                this.consumableIconField = value;
            }
        }

        /// <remarks/>
        public string ConsumableUniqueID
        {
            get
            {
                return this.consumableUniqueIDField;
            }
            set
            {
                this.consumableUniqueIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class Capacity
    {

        private byte maxCapacityField;

        private string unitField;

        /// <remarks/>
        public byte MaxCapacity
        {
            get
            {
                return this.maxCapacityField;
            }
            set
            {
                this.maxCapacityField = value;
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class ConsumableLifeState
    {

        private string consumableStateField;

        private string measuredQuantityStateField;

        private string brandField;

        private bool isRefilledField;

        private bool isSETUPField;

        private bool isTrialField;

        /// <remarks/>
        public string ConsumableState
        {
            get
            {
                return this.consumableStateField;
            }
            set
            {
                this.consumableStateField = value;
            }
        }

        /// <remarks/>
        public string MeasuredQuantityState
        {
            get
            {
                return this.measuredQuantityStateField;
            }
            set
            {
                this.measuredQuantityStateField = value;
            }
        }

        /// <remarks/>
        public string Brand
        {
            get
            {
                return this.brandField;
            }
            set
            {
                this.brandField = value;
            }
        }

        /// <remarks/>
        public bool IsRefilled
        {
            get
            {
                return this.isRefilledField;
            }
            set
            {
                this.isRefilledField = value;
            }
        }

        /// <remarks/>
        public bool IsSETUP
        {
            get
            {
                return this.isSETUPField;
            }
            set
            {
                this.isSETUPField = value;
            }
        }

        /// <remarks/>
        public bool IsTrial
        {
            get
            {
                return this.isTrialField;
            }
            set
            {
                this.isTrialField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class ConsumableStateAvailableActions
    {

        private string consumableStateField;

        private string[] supportedActionsField;

        private string defaultActionField;

        /// <remarks/>
        public string ConsumableState
        {
            get
            {
                return this.consumableStateField;
            }
            set
            {
                this.consumableStateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ConsumableStateAction", IsNullable = false)]
        public string[] SupportedActions
        {
            get
            {
                return this.supportedActionsField;
            }
            set
            {
                this.supportedActionsField = value;
            }
        }

        /// <remarks/>
        public string DefaultAction
        {
            get
            {
                return this.defaultActionField;
            }
            set
            {
                this.defaultActionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class RegionalCartridge
    {

        private byte regionIdentifierField;

        /// <remarks/>
        public byte RegionIdentifier
        {
            get
            {
                return this.regionIdentifierField;
            }
            set
            {
                this.regionIdentifierField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/", IsNullable = false)]
    public partial class ConsumableIcon
    {

        private string shapeField;

        private string rotationField;

        private ConsumableIconFillColor fillColorField;

        private ConsumableIconOutlineColor outlineColorField;

        private ConsumableIconBackGroundColor backGroundColorField;

        /// <remarks/>
        public string Shape
        {
            get
            {
                return this.shapeField;
            }
            set
            {
                this.shapeField = value;
            }
        }

        /// <remarks/>
        public string Rotation
        {
            get
            {
                return this.rotationField;
            }
            set
            {
                this.rotationField = value;
            }
        }

        /// <remarks/>
        public ConsumableIconFillColor FillColor
        {
            get
            {
                return this.fillColorField;
            }
            set
            {
                this.fillColorField = value;
            }
        }

        /// <remarks/>
        public ConsumableIconOutlineColor OutlineColor
        {
            get
            {
                return this.outlineColorField;
            }
            set
            {
                this.outlineColorField = value;
            }
        }

        /// <remarks/>
        public ConsumableIconBackGroundColor BackGroundColor
        {
            get
            {
                return this.backGroundColorField;
            }
            set
            {
                this.backGroundColorField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class ConsumableIconFillColor
    {

        private byte redField;

        private byte greenField;

        private byte blueField;

        /// <remarks/>
        public byte Red
        {
            get
            {
                return this.redField;
            }
            set
            {
                this.redField = value;
            }
        }

        /// <remarks/>
        public byte Green
        {
            get
            {
                return this.greenField;
            }
            set
            {
                this.greenField = value;
            }
        }

        /// <remarks/>
        public byte Blue
        {
            get
            {
                return this.blueField;
            }
            set
            {
                this.blueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class ConsumableIconOutlineColor
    {

        private byte redField;

        private byte greenField;

        private byte blueField;

        /// <remarks/>
        public byte Red
        {
            get
            {
                return this.redField;
            }
            set
            {
                this.redField = value;
            }
        }

        /// <remarks/>
        public byte Green
        {
            get
            {
                return this.greenField;
            }
            set
            {
                this.greenField = value;
            }
        }

        /// <remarks/>
        public byte Blue
        {
            get
            {
                return this.blueField;
            }
            set
            {
                this.blueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hp.com/schemas/imaging/con/dictionaries/1.0/")]
    public partial class ConsumableIconBackGroundColor
    {

        private byte redField;

        private byte greenField;

        private byte blueField;

        /// <remarks/>
        public byte Red
        {
            get
            {
                return this.redField;
            }
            set
            {
                this.redField = value;
            }
        }

        /// <remarks/>
        public byte Green
        {
            get
            {
                return this.greenField;
            }
            set
            {
                this.greenField = value;
            }
        }

        /// <remarks/>
        public byte Blue
        {
            get
            {
                return this.blueField;
            }
            set
            {
                this.blueField = value;
            }
        }
    }



}
