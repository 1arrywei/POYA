﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using POYA.Unities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
    #region 

    /*
    public class EVideo //  ViewModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UseFileId { get; set; }
        public string UserId { get; set; }
        //  public Guid ArticleId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DOPublish { get; set; }
    }
    */

    #endregion

    public class EArticle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        #region USER

        public string UserId { get; set; }

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public string UserName { get; set; }
        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public string UserEmail { get; set; }

        #endregion

        #region EARTICLE_SET

        #region 
        /// <summary>
        /// The EArticle set id
        /// </summary>
        #endregion
        public Guid SetId { get; set; }     //  = LValue.DefaultEArticleSetId;

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public string SetName { get; set; }

        #endregion

        #region CATEGORY

        #region 

        /// <summary>
        /// CategoryId include the first and second category, it is  reference from GB/T 13745-2009,
        /// <br/> and there's no guarantee that they're fully compatible,
        /// category of earticle is just a label which author want to set, 
        /// <br/>please let us know if you have a better category 
        /// </summary>
        #endregion
        public Guid CategoryId { get; set; }

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public List<SelectListItem> FirstCategorySelectListItems { get; set; }

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public List<SelectListItem> SecondCategorySelectListItems { get; set; }

        public string AdditionalCategory { get; set; }

        #region 
        [Range(0,3)]
        #endregion
        public int ComplexityRank { get; set; } = 0;

        #region 

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public List<SelectListItem> ComplexityRankSelectListItems { get; set; }

        #endregion

        #region 

        [StringLength(maximumLength: 50, MinimumLength = 2)]
        #endregion
        public string Title { get; set; }

        #region 

        [StringLength(maximumLength: 16384)]
        #endregion
        public string Content { get; set; }

        #region 

        /// <summary>
        /// Determine the article is legal or not by Content appraiser, the default value is <see langword="true"/>
        /// </summary>
        #endregion
        public bool IsLegal { get; set; } = true;

        public DateTimeOffset DOPublishing { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? DOUpdating { get; set; }

        public long ClickCount { get; set; } = 0;

        #region DEPOLLUTION

        #region 

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public long ReaderCount { get; set; } = 0;

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>

        [NotMapped]
        #endregion

        public IEnumerable<IFormFile> LVideos { get; set; }

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public IEnumerable<IFormFile> LAttachments { get; set; }
        #endregion

        #region DISCARD

        #region 

        /// <summary>
        /// || DISCARD ||
        /// </summary>
        #endregion
        public Guid ClassId { get; set; }

        #region 

        /// <summary>
        /// || DISCARD ||
        /// </summary>
        #endregion
        public Guid LGradeId { get; set; }

        #region 

        /// <summary>
        /// || DISCARD ||
        /// "text/html" or "text/markdown", the default is "text/html"
        /// </summary>
        #endregion
        public string ContentType { get; set; } //    = "text/html";
        #endregion
    }

    public class EArticleFile
    {
        #region 

        /// <summary>
        /// The default is  Guid.NewGuid()
        /// </summary>
        #endregion
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EArticleId { get; set; }
        public string FileSHA256 { get; set; }
        public string FileName { get; set; }
        public bool IsEArticleVideo { get; set; } = false;
        #region DEPOLLUTION

        #region 

        /// <summary>
        /// [NotMapped]
        /// </summary>
        #endregion
        [NotMapped]
        public string ContentType { get; set; }
        #endregion
    }

    public class EArticleFileSHA256
    {
        public Guid EArticleId { get; set; }
        public string FileName { get; set; }
        public string SHA256 { get; set; }
        public bool IsEArticleVideo { get; set; } = false;
    }

    #region DEPOLLUTION

    public class LEArticleCategory
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    #region ARTICLE_CLASS
    /*
     * Id,Code,Name

     * 
    public class LField
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [StringLength(maximumLength:20)]
        public string Name { get; set; }
    }
    public class LAdvancedClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LFieldId { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
    }
    public class LSecondaryClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LAdvancedClassId { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
    }
    public class LGrade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LSecondaryClassId { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
    }
    public class LGradeComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LGradeId { get; set; }
        [StringLength(maximumLength: 50)]
        public string Comment { get; set; }

    }
    */
    #endregion


    #endregion
}
