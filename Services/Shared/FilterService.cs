using AddressBookManagement.Models;
using System.Linq.Expressions;

namespace AddressBookManagement.Services.Shared
{
    public class FilterService
    {
        public List<Expression<Func<Contact, bool>>> Build(ContactFilter filter)
        {
            var expressions = new List<Expression<Func<Contact, bool>>>();

            //Filter fullname
            if (!string.IsNullOrWhiteSpace(filter.NameFilter))
            {
                var nameFilter = filter.NameFilter.ToLower();
                expressions.Add(c =>
                    ((c.FirstName ?? "").ToLower() + " " + (c.LastName ?? "").ToLower())
                        .Contains(nameFilter)
                );
            }

            //Filter job title
            if (!string.IsNullOrWhiteSpace(filter.JobTitleFilter))
                expressions.Add(c => (c.JobTitle ?? "").Contains(filter.JobTitleFilter));

            //Filter organization
            if (filter.OrganizationFilter.HasValue)
                expressions.Add(c => c.OrganizationId == filter.OrganizationFilter.Value);

            //Filter group
            if (filter.GroupFilter.HasValue)
                expressions.Add(c => c.Group == filter.GroupFilter.Value);

            //Filter relationship
            if (filter.RelationshipFilter.HasValue)
                expressions.Add(c => c.Relationship == filter.RelationshipFilter.Value);

            return expressions;
        }
    }
}
