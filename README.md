# MongoDB Repository
### by [Benjamin Abt](http://www.benjamin-abt.com) - 2015, 8. March

## Project Description
This library is currently the easiest way to interact with MongoDB by using .NET.

It will give you the most flexible and powerful possibility to securely and safely create, delete, read, change, and manage your entities and queries with the MongoDB.

This is a completely new written project of my existing NuGet package [MongoDB Infrastructure - Repository and Entity Base](https://www.nuget.org/packages/SchwabenCode.MongoDBInfrastructure/)

## Repository Pattern
For further information about the Repository Pattern have a look at the [https://msdn.microsoft.com/en-us/library/ff649690.aspx](MSDN).

## Features

#### Official C# Driver by 10gen
In the background, the official MongoDB C# Driver is used.

#### .NET 4.0 Framework
Project is developed with the focus on .NET 4.0 and higher.

#### Async
All methods are also available as an asynchronous implementation, too.
While MongoDB supports asynchronous not natively, [AsyncAll by SchwabenCode](https://www.nuget.org/packages/AsyncAll/) is used.

#### CRUD
CRUD methods are all available and overridable.

#### Typed entities and raw BsonDocuments are supported
CRUD support for typed objects (own entity classes) as well as BSON documents.

#### Validation
A validation pattern can be added to the entities. The usage will be nearly similar to the entity framework.
Every entity will be validated before writing. On error an exception will be thrown.

For a very simple implementation and integration in existing systems two different repositories are provided - with and without validation.

#### Discoverable Entities
With the help of appropriate interfaces you can read an entity completely or only individual fields.
As opposed to the standard c# driver you can define appropriate part models and you do not have to touch the repository logic.

To maximize the performance and minimize the usage of reflection all metadata information will be cached.

#### Testability and modularization
Full integration of dependency injection at all levels.

#### Code Contracts
All classes and methods support [Microsoft code contracts](https://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970) for incoming parameters as well as for methods returns.

## Usage

#### Models
First of all create an empty interface for your model and implement *IMongoEntity*.  
With the empty interface you create a scope for your model and partial models.
<pre><code>/// &lt;summary&gt;
/// This interface is used for all address models.
/// This main interface may have held no implementation of the properties, since otherwise no part models can be created.
/// &lt;/summary&gt;
public interface IAddress : IMongoEntity
{
    // Leave this empty
}</code></pre>

Now create your model and let it implement your interface, here *IAddress*.  
Due to the *IAddress* is implemnenting IMongoEntity your class implements *IMongoEntity* automatically, too.
<pre><code>/// &lt;summary&gt;
/// This class represents the full entity for 'Address'. Here, all properties are defined.
/// It must implement the interface 'IAddress' because our AddressRepository takes only IAddress objects.
/// &lt;/summary&gt;
public class Address : IAddress
{
    public ObjectId ID { get; set; }

    public String Label { get; set; }
    public String Street { get; set; }
    public String ZipCode { get; set; }
    public String City { get; set; }
    public String Country { get; set; }
}</code></pre>

##### Optional
If required or wanted you can implement a validation thats works nearly similar like the validation of the Entity Framework:  
on every write (*Add*, *Update*) the repository will execute the validation implementation and throws an *MongoInvalidEntityException* if the validation fails.

The interface would look like
<pre><code>/// &lt;summary&gt;
/// This interface is used for all person models and supports validation.
/// This main interface may have held no implementation of the properties, since otherwise no part models can be created.
/// &lt;/summary&gt;
public interface IPerson : IMongoEntityValidatable
{
    // Leave this empty
}</code></pre>

and entity looks like
<pre><code>public class Person : MongoEntityValidatable, IPerson
{
    public ObjectId UserID { get; set; }

    public String Name { get; set; }
    public String EMail { get; set; }

    /// &lt;summary&gt;
    /// Custom Entity Validation
    /// &lt;/summary&gt;
    /// &lt;returns&gt;&lt;/returns&gt;
    public override IEnumerable&lt;ValidationResult&gt; Validate( )
    {
        if ( !String.IsNullOrEmpty( Name ) )
        {
            yield return new ValidationResult( &quot;Required Property 'Name' is missing.&quot;, new[ ] { &quot;Name&quot; } );
        }

        if ( !String.IsNullOrEmpty( EMail ) )
        {
            yield return new ValidationResult( &quot;Required Property 'EMail' is missing.&quot;, new[ ] { &quot;EMail&quot; } );
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
                result = new ValidationResult( &quot;Entered EMail Address is invalid.&quot;, new[ ] { &quot;EMail&quot; } );
            }
            if ( result != null )
            {
                yield return result;
            }
        }
    }
}</code></pre>

#### Partial or discoverable models
Now, this is the reason why we have to use empty interfaces.

In most cases, you won't need not all fields to read. So why load unnecessary fields?
Take the better way!

First of all, create a new class with only the required fields of your MongoDB entity.
In this example we just want to load the address label.
<pre><code>public class AddressLabel : IMongoDiscoverable, IAddress
{
    public String Label { get; set; }
}</code></pre>
We still have to implement the *IAddress* to let the repository know, that we still are in the scope of your *Address*.
But we also have to implement *IMongoDiscoverable*.  

The repository checks on every read method if the class implements this interfaces. If yes it determines all properties and uses the *SetFields* method of the MongoDB C# Driver to only load the required fields.  
In this case *SetFields( "Label" )* would be executed.

You can use this interface for all classes, but it's better for your performance to use this way only if you are working with partial models.
In the background a cached reflection algorithm is implemented.  
The overhead takes place only at the first time you are using a partial model type.
#### Repository
For our Address we were using the Repository without the validation. So we have to inherit from *MongoRepository* instead of *MongoValidatableRepository*.  
This would look like: 
<pre><code>    public class AddressRepository : MongoRepository&lt;Address, IAddress&gt; // No validation!
    {
        public AddressRepository( IMongoUnitOfWork uow )
            : base( uow, idFieldName: MongoDiscoverer.GetFieldName&lt;Address&gt;( address =&gt; address.ID ) /* this returns &quot;ID&quot; */ )
        {

        }
    }</code></pre>

Yes, you still have to set some variables. The repository has to know what your ID field is.  
So please override the required *IDFieldName* or several methods wont work. But: That's all!

The *TEntity* generic type (here passed with *Address*) has to be the full entity type.  
All methods without generic support (like *GetAll( )*) will use this type.  

The *IEntity* generic type (here passed with *IAddress*) has to be the interface scope. It <u>must</u> be an interface, our you'll cannot use the benefits!  
All methods with generic support (like *GetAll&lt;AddressLabel&gt;*) will use this this as type constraint.

As well, a much more simple usage of the repository pattern is available:
<pre><code>    // ######### Examples - Persons - without specific Repository
    // you have to set there the id field in the constructor
    var personDefaultRepository = new MongoRepository&lt;Person&gt;( uowContainer, 
                    MongoDiscoverer.GetFieldName&lt;Person&gt;( p =&gt; p.UserID )  /* this returns &quot;UserID&quot; */ );</code></pre>

or without interface scoping

<pre><code>    public class PersonRepositoryNoInterface : MongoRepository&lt;Person&gt; // Without interface options
    {
        /// &lt;summary&gt;
        /// Creates an instance for a repository without the using of an interace
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;uow&quot;&gt;Attached Unit of Work Container&lt;/param&gt;
        public PersonRepositoryNoInterface( IMongoUnitOfWork uow )
            : base( uow, MongoDiscoverer.GetFieldName&lt;Person&gt;( p =&gt; p.UserID /* this returns &quot;UserID&quot; */ )
                /* avoid magic strings! */,
                    &quot;PersonsSecondCollection&quot; /* but u should avoid hard coded collection names */)
        {
        }
    }</code></pre>


Now you can use the repository in this default state or you can implement custom query methods.  
Feel free.

#### Repository Usage
A sample code snippet says more than thousand words:
<pre><code>AddressRepository addressRepository = new AddressRepository( uowContainer );

// ### Get All
// Read all addresses with full entity details (uses default repository type)
var addressesFull = addressRepository.GetAll( );
// or with type
var addressesFullByType = addressRepository.GetAll&lt;Address&gt;( );
// or only specific fields
var addessLabels = addressRepository.GetAll&lt;AddressLabel&gt;( );</code></pre>

#### Demo
Take a look at the full demo application *AddressManager* in the source.

## Give Thanks
It took many hours to create this library in its published form.  
If you like the library and saved much time than maybe respect this with a small donation to the [animal shelter of Stuttgart](http://www.tierheim-stuttgart.de/).

If you want to thank me personally take a look at [my personal Amazon wishlist](http://www.amazon.de/gp/registry/wishlist/H6KLKT7UMI7Z/).

It would be also very nice when you just write me if you like this implementation and tell me what you've started!

#### Versions
* 1.0.0.0 - 08.03.2015 - Initial Release 

## License
The MIT License (MIT)

Copyright (c) 2015 Benjamin Abt - www.benjamin-abt.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


## Remarks
All trademarks are the property of their respective owners.

This library was created on the basis of my own needs.
I am not responsible for integration issues, errors or any damage. If you have problems, please use public forums.
For errors please use the issue tab of this project site.

Thank you and good luck with your software.