//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle. @04/13/2018 09:43:47
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MMPModel
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Représente un utilisateur du système
    /// </summary>
    public partial class User : IEntity
    {
    
        #region Constructeur
    
    	/// <summary>
    	/// Initialise un nouvel objet User
    	/// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.CreditScore = 0;
            this.WelcomeEmailSent = false;
        }

        #endregion

        #region Propriétés simples
    
    	/// <summary>
        /// L&apos;identifiant unique de l&apos;utilisateur
        /// </summary>
        public System.Guid Id { get; set; }
    
    	/// <summary>
        /// Le nom de l&apos;utilisateur
        /// </summary>
        public string Name { get; set; }
    
    	/// <summary>
        /// La date de création de l&apos;utilisateur dans le système
        /// </summary>
        public System.DateTime CreatedOn { get; set; }
    
    	/// <summary>
        /// Le nombre de crédit de l&apos;utilisateur
        /// </summary>
        public int CreditScore { get; set; }
    
    	/// <summary>
        /// L&apos;adresse email de l&apos;utilisateur
        /// </summary>
        public string Email { get; set; }
    
    	/// <summary>
        /// Est-ce que le mail de bienvenue a été envoyé ?
        /// </summary>
        public bool WelcomeEmailSent { get; set; }

        #endregion

    }
}