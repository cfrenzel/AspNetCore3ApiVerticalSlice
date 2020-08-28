
 
 
- Uses a json file to seed an embedded sqlLite database.  Use migrations to create the database on startup

- Uses a vertical slice architecture to keep controllers, models, logic tightly together per action
   - Use MediatR to simple CQRS (command / query).  Also gives a great place to hook in pipelines
     to perform any needed logic before or after handling the command/query (ex. validaton/logging)
   
   - Allows full control of models for every endpoint (or they can be shared if desired)
        - no overposting
        - clean interface between api models and efcore/entities
        - can query using any technology for optimal performance
            - efcore 
            - efcore with AutoMapper.QueryableExtensions (see List Handlers / ProjectTo)
            - dapper for complex queries
            - or even get data from a third party
        - AutoMapper when needing to cleanly go between Entities and Models (Dto)

   - We could add a more generic/advanced base return type to our handlers to get all kinds
      of cool built-in behavior based on our needs
      - A cleaner approach would be to return IActionResult from the handler.
        Your controllers would basically be empty! We could write a little miiddelware to avoid controllers completely :)
        For now we'll keep it recogniizable as a conventional mvc app
    
- And
   - Microsoft.AspNetCore.Mvc.Versioning - for flexible out of the box versioning
   - FluentValidation
   - Simiple Paging
   - Dependency Injection throughout
   - https://github.com/khellang/Middleware / app.UseProblemDetails();
        - for some good defaults around errors and debugging
   - simple example of optimistic concurrency for the account transfer


- To keep it simple from a REST perspective we're not being very strict
    - No Authentication
    - no nested resources
    - Using "actions" / RPC like behavior when convenient
    - Singular endpoint naming
    - No HATEOAS
    - No Content Negotiaton (though it would be easy to add)
   
-
TODO:
- api error handling
- add swagger
- etag concurrency: https://blog.jeremylikness.com/blog/2017-12-15_five-restful-web-design-patterns-implemented-in-asp.net-core-2.0-part-4-optimistic-concurrency/
- better error messages from handler (account balance, etc...)
