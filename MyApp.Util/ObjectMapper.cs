
using MyApp.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Util
{
    public static class ObjectMapper
    {
        public static void MapSourceObjectToTarget<TSource, TTarget>(TSource dto, TTarget dataModel,bool IgnoreIfNotFoundInTarget=false)
        {

            var listOfProperties = dto!.GetType().GetProperties();
            var propertyList = (from prop in listOfProperties
                                let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                                select prop.Name).ToList();
            foreach(var prop in propertyList)
            {
                if (dataModel!.GetType().GetProperty(prop) == null && IgnoreIfNotFoundInTarget == false)
                {
                    throw new CustomException($"Property {prop} not found in {dto.GetType}");
                }
                if (dataModel.GetType().GetProperty(prop) != null)
                {
                    if (prop != "CaptchaResponse")
                    {
                        if ((dataModel.GetType().GetProperty(prop)!.PropertyType == typeof(TimeOnly)|| dataModel.GetType().GetProperty(prop)!.PropertyType == typeof(TimeOnly?)) && dto.GetType().GetProperty(prop)!.PropertyType==typeof(string)) // Conversion is required from string to TimeOnly
                        {
                            var propValue = dto.GetType().GetProperty(prop)!.GetValue(dto);
                            if (propValue == null)
                            {
                                dataModel.GetType().GetProperty(prop)!.SetValue(dataModel, null);
                            }
                            else
                            {
                                dataModel.GetType().GetProperty(prop)!.SetValue(
                                dataModel, TimeOnly.ParseExact(dto.GetType().GetProperty(prop)!.GetValue(dto)!.ToString()!, "HH:mm")
                                );
                            }
                        }
                        else if ((dataModel.GetType().GetProperty(prop)!.PropertyType == typeof(DateOnly) || dataModel.GetType().GetProperty(prop)!.PropertyType == typeof(DateOnly?)) && dto.GetType().GetProperty(prop)!.PropertyType == typeof(string)) // Conversion is required from string to TimeOnly
                        {
                            var propValue = dto.GetType().GetProperty(prop)!.GetValue(dto);
                            if (propValue == null)
                            {
                                dataModel.GetType().GetProperty(prop)!.SetValue(dataModel,null);
                            }
                            else
                            {
                                dataModel.GetType().GetProperty(prop)!.SetValue(dataModel,
                                    DateOnly.ParseExact(dto.GetType().GetProperty(prop)!.GetValue(dto)!.ToString()!, "d-M-yyyy"));
                            }
                        }
                        else
                        {
                            dataModel.GetType().GetProperty(prop)!.SetValue(dataModel, dto.GetType().GetProperty(prop)!.GetValue(dto));
                        }
                    }
                }
            }
            //propertyList.ForEach(prop =>
            //{
            //    dataModel.GetType().GetProperty(prop).SetValue(
            //        dataModel, dto.GetType().GetProperty(prop).GetValue(dto)
            //        );
            //});
        }

        public static List<string> GetListOfProperties(Type t)
        {
            
            var listOfProperties = t.GetProperties();// typeof(TModel).GetProperties();
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }

        ///*
        //    This method can be used to Serialize Object to JSON string.
        //    Source : Paypal
        //*/
        //public static string ObjectToJSONString(object serializableObject)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    var writer = JsonReaderWriterFactory.CreateJsonWriter(
        //                memoryStream, Encoding.UTF8, true, true, "  ");
        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
        //    ser.WriteObject(writer, serializableObject);
        //    memoryStream.Position = 0;
        //    StreamReader sr = new StreamReader(memoryStream);
        //    return sr.ReadToEnd();
        //}
    }
}
