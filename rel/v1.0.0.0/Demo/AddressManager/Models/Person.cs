using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;
using MongoDB.Bson;
using SchwabenCode.MongoDBRepository;

namespace AddressManager.Models
{
    public class Person : MongoEntityValidatable, IPerson
    {
        public ObjectId UserID { get; set; }

        public String Name { get; set; }
        public String EMail { get; set; }

        /// <summary>
        /// Custom Entity Validation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ValidationResult> Validate( )
        {
            if ( !String.IsNullOrEmpty( Name ) )
            {
                yield return new ValidationResult( "Required Property 'Name' is missing.", new[ ] { "Name" } );
            }

            if ( !String.IsNullOrEmpty( EMail ) )
            {
                yield return new ValidationResult( "Required Property 'EMail' is missing.", new[ ] { "EMail" } );
            }
            else
            {
                // Verify EMails not with regex, use MailAddress class!
                // See https://msdn.microsoft.com/en-us/library/01escwtf.aspx

                ValidationResult result = null;
                try
                {
                    MailAddress m = new MailAddress( EMail );
                }
                catch ( FormatException )
                {
                    result = new ValidationResult( "Entered EMail Address is invalid.", new[ ] { "EMail" } );
                }
                if ( result != null )
                {
                    yield return result;
                }
            }
        }
    }
}
