using AutoMapper;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentApplication.Models.Models;

namespace Znode.Multifront.PaymentApplication.Api
{
    public static class AutoMapperConfig
    {
        public static void Execute()
        {
            Mapper.CreateMap<ZNodePaymentType, PaymentTypeModel>();
            Mapper.CreateMap<ZNodePaymentGateway, PaymentGatewayModel>();
            Mapper.CreateMap<PaymentModel, ZnodePaymentMethod>()
                .ForMember(d => d.CreditCardExpMonth, opt => opt.MapFrom(src => src.CardExpirationMonth))
                .ForMember(d => d.PaymentSettingID, opt => opt.MapFrom(src => src.PaymentApplicationSettingId))
                 .ForMember(d => d.CreditCardExpYear, opt => opt.MapFrom(src => src.CardExpirationYear)).ReverseMap();
            Mapper.CreateMap<ZNodePaymentSetting, PaymentSettingsModel>();
            Mapper.CreateMap<PaymentSettingsModel, ZNodePaymentSetting>();
            Mapper.CreateMap<PaymentSettingsModel, ZNodePaymentSettingCredential>();
            Mapper.CreateMap<ZNodePaymentSettingCredential, PaymentSettingCredentialsModel>();
            Mapper.CreateMap<ZnodeTransaction, PaymentTransactionModel>()
                 .ForMember(d => d.CCToken, opt => opt.MapFrom(src => src.Custom1));
            Mapper.CreateMap<ZnodeAuthToken, ZnodeAuthTokenModel>();
        }
    }
}