using DeliveryBot.Application.Abstractions;
using DeliveryBot.Application.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryBot.Application.Services
{
    public class LanguageRequestModelService 
    {
#pragma warning disable
        public LanguageResponseModel LanguageResponse(string languageCode)
        {
            var model = new LanguageResponseModel();
            if (languageCode.ToLower() == "uzbek")
            {

                model.Hello = "Assalomu alaykum";
                model.Order = "Buyurtma berish";
                model.About_us = "Biz haqimizda";
                model.Uz = "O'zbek tili";
                model.En = "Ingliz tili";
                model.Ru = "Rus tili";
                model.Change_language = "Tilni o'zgartirish";
                model.Choose_language = "Tilni tanlamoq";
                model.Back = "Orqaga";
                model.LanguageChangeSuccess = "Muffaqiyatli o'zgardi";

            }
            else
            {
                return model;
            }
            return model;
        }
    }
}
