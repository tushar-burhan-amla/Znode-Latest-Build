using System.ComponentModel.DataAnnotations;
namespace Znode.Engine.WebStore.ViewModels
{
    public class WebGridColumnViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Headertext { get; set; }
        [Required]
        public string Columntype { get; set; }
        public int Width { get; set; }
        public bool Allowsorting { get; set; }
        public bool Allowpaging { get; set; }
        public string Format { get; set; }

        public char Isvisible { get; set; }

        public char Mustshow { get; set; }

        public char Musthide { get; set; }
        public string Datatype { get; set; }
        public int Maxlength { get; set; }

        public char Isallowsearch { get; set; }

        public char Isconditional { get; set; }

        public char Isallowlink { get; set; }

        public string Islinkactionurl { get; set; }

        public string Islinkparamfield { get; set; }

        public string Displaytext { get; set; }
        //new parameters
        public string Editactionurl { get; set; }
        public string Editparamfield { get; set; }

        public string Deleteactionurl { get; set; }
        public string Deleteparamfield { get; set; }

        public string Manageactionurl { get; set; }
        public string Manageparamfield { get; set; }

        public string Viewactionurl { get; set; }
        public string Viewparamfield { get; set; }

        public string Imageactionurl { get; set; }
        public string Imageparamfield { get; set; }

        public string Copyactionurl { get; set; }
        public string Copyparamfield { get; set; }

        public char Ischeckbox { get; set; }
        public string Checkboxparamfield { get; set; }

        public char Iscontrol { get; set; }
        public string Controltype { get; set; }
        public string Controlparamfield { get; set; }
        public char XAxis { get; set; }
        public char YAxis { get; set; }
        public char IsAdvanceSearch { get; set; }
        public string Class { get; set; }
        public string DbParamField { get; set; }
        public string SearchControlType { get; set; }
        public string SearchControlParameters { get; set; }
        public string IsGraph { get; set; }
        public string UseMode { get; set; }
        public char AllowDetailView { get; set; }
    }
}