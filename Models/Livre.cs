﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace GPLCC_2.Models
{
    public partial class Livre
    {
        public Livre()
        {
            Pret = new HashSet<Pret>();
        }

        [Display(Name = "Numéro du livre")]
        public int Numero { get; set; }
        [Required(ErrorMessage = @"Le Titre est requis")]
        public string Titre { get; set; }
        [Required(ErrorMessage = @"L'Auteur est requis")]
        public string Auteur { get; set; }

        [NotMapped]
        public string Affichage { get {
                return Numero + " - " + Titre;
            }
             set { Affichage = value; }
        }

        [Display(Name = "Catégorie")]
        public int Categories { get; set; }

        public virtual ICollection<Pret> Pret { get; set; }
    }
}