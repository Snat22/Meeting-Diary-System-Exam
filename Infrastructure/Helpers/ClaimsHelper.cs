using System.Reflection;
using Domain.DTOs.RolePermissionDTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Helpers;

public static class ClaimsHelper
{
    public static void GetPermissions(this List<RoleClaimsDto> allPermissions, Type policy)
    {
        var nestedTypes = policy.GetNestedTypes(BindingFlags.Public);
        if (nestedTypes.Length > 0)
        {
            foreach (var nested in nestedTypes)
            {
                FieldInfo[] fields = nested.GetFields(BindingFlags.Static | BindingFlags.Public);

                foreach (FieldInfo f in fields)
                {
                    allPermissions.Add(new RoleClaimsDto("Permissions", f.GetValue(null).ToString()));
                }
            }
        }
        else
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo f in fields)
            {
                allPermissions.Add(new RoleClaimsDto(f.GetValue(null).ToString(), "Permissions"));
            }
        }
    }



    public static async Task AddPermissionClaim(this DataContext context, Role role, string permission)
    {

        var allClaim = await context.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();
        if (!allClaim.Any(x => x.ClaimType == "Permissions" && x.ClaimType == permission))
        {
            await context.RoleClaims.AddAsync(new RoleClaim()
            {
                ClaimType = "Permissions",
                Role = role,
                RoleId = role.Id,
                ClaimValue = permission,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
            });
            await context.SaveChangesAsync();
        }
    }



}
