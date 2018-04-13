﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle. @04/13/2018 09:46:15
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MMPModel.Service
{
    using System;
    using System.Collections.Generic;
    using Mehdime.Entity;
    
    using MMPModel.Repository;
    
    /// <summary>
    /// Classe de base pour tous les services
    /// </summary>
    public abstract class BaseService
    {
    	
    	protected readonly IDbContextScopeFactory _dbContextScopeFactory;
        
        internal protected BaseService(IDbContextScopeFactory dbContextScopeFactory)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException("dbContextScopeFactory");
            _dbContextScopeFactory = dbContextScopeFactory;
        }
    
        #region Accès simplifié aux repositories
    
    	private static UserRepository _UserRepos;
        internal UserRepository UserRepository
        {
            get { return _UserRepos ?? (_UserRepos = new UserRepository()); }
        }

        #endregion

    }
}