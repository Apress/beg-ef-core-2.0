# Errata for *Beginning Entity Framework Core 2.0 *

On **page 116** [In Listing 3-10, an error occurs in the line "foreach(var os in lOs)"]:
 
System.InvalidOperationException
  HResult=0x80131509
  Message=The LINQ expression '(GroupByShaperExpression:
KeySelector: (m.OperatingSysID),
ElementSelector:new { 
    m = (EntityShaperExpression: 
        EntityType: Machine
        ValueBufferExpression: 
            (ProjectionBindingExpression: m)
        IsNullable: False
    ), 
    o = (EntityShaperExpression: 
        EntityType: OperatingSys
        ValueBufferExpression: 
            (ProjectionBindingExpression: o)
        IsNullable: False
    )
 }
)
    .Select(x => x.m.OperatingSysId)' could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to either AsEnumerable(), AsAsyncEnumerable(), ToList(), or ToListAsync(). 

See https://docs.microsoft.com/en-us/ef/core/querying/client-eval for more information.

***