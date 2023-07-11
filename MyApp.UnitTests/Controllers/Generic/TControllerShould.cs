using Microsoft.AspNetCore.Mvc;
using MyApp.API.Controllers;
using MyApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MyApp.Tests.Controllers
{
    public class TControllerShould<Tdto,Tmodel> where Tdto:class where Tmodel : class
    {
        private readonly ITestOutputHelper output;
        private readonly List<string> testPropertyName;

        private GenericController<Tdto,Tmodel> GetController()
        {
            var conn = Settings.Get_IDbConnection_Object();
            GenericController<Tdto, Tmodel> controller = new(new GenericRepository<Tmodel>(conn, Settings.GetMockAccessor_SuperAdmin()));
            controller = Settings.AttachControllerContext_Admin(controller);
            return controller;
        }

        public  TControllerShould(ITestOutputHelper output,string PropertyNamesCSV)
        {
            this.output = output;
            testPropertyName = PropertyNamesCSV.Split(",").ToList();
        }
        
        public virtual void GetAll()
        {
            GenericController<Tdto, Tmodel> controller = GetController();
            ObjectResult objectResult = (ObjectResult)(controller.GetAll());
            List<Tmodel>? list = (List<Tmodel>?)objectResult.Value;
            Assert.True(list?.Count > 0);
        }
        
        public virtual void Get()
        {
            GenericController<Tdto, Tmodel> controller = GetController();
            ObjectResult objectResult = (ObjectResult)(controller.Get(1));
            Tmodel? outputModel = (Tmodel?)objectResult.Value;
            Assert.True(outputModel?.GetType().GetProperty(testPropertyName[0])!=null);
        }

        // Test the text-properties passed in PropertyNamesCSV with dummy text value
        public virtual void Post_Put_Delete() 
        {
            //AccountControllerShould.LoginSuccessfully();
            GenericController<Tdto, Tmodel> controller = GetController();

            // POST
            Tdto dto = (Tdto)Activator.CreateInstance(typeof(Tdto))!;
            SetProperties(typeof(Tdto), dto, "T");
            ObjectResult objectResult = (ObjectResult)controller.Post(dto);
            Tmodel? createdModel = (Tmodel?)objectResult.Value;
            int createdId = (int)typeof(Tmodel).GetProperty("Id")!.GetValue(createdModel)!;
            Assert.True(createdId > 0);
            output.WriteLine("POST Succeded at " + DateTime.Now);

            // PUT
            Tmodel model = (Tmodel)Activator.CreateInstance(typeof(Tmodel))!;
            typeof(Tmodel).GetProperty("Id")!.SetValue(model, createdId);
            SetProperties(typeof(Tmodel), model, "TEdit");
            NoContentResult putResult = (NoContentResult)(controller.Put(model));
            Assert.True(putResult!=null);
            output.WriteLine("PUT Succeded at " + DateTime.Now);

            // DELETE
            objectResult = (ObjectResult)(controller.Get(createdId));
            Tmodel? getModel = (Tmodel?)objectResult.Value;
            Assert.True((string)typeof(Tmodel).GetProperty(testPropertyName[0])!.GetValue(getModel)! == "TEdit");
            output.WriteLine("PUT and GET Succeded at " + DateTime.Now);
            NoContentResult deleteResult = (NoContentResult)controller.Delete(createdId);
            Assert.True(deleteResult!=null);
            output.WriteLine("DELETE Succeded at " + DateTime.Now);
        }

        // Assuming all properties are text
        private void SetProperties(Type T, Object o,string Value)
        {
            foreach(var item in testPropertyName)
            {
                T.GetProperty(item)!.SetValue(o, Value);
            }
        }
    }
}
