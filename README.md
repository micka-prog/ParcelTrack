# ParcelTrack (starter)

CWEB2302 Web Development II, Common Coursework (Individual).

## Run it
1. Open `ParcelTrack.csproj` in Visual Studio.
2. Package Manager Console: `Update-Database`.
3. F5 (the **http** profile, http://localhost:5420), then open `/parcels`. Five parcels show.

## Demo accounts (made when the app starts)
| Email | Password | Role |
|---|---|---|
| admin@parceltrack.test | Pass123! | Admin |
| staff@parceltrack.test | Pass123! | Staff |

## What the starter already does
- Parcels and Branches tables (a foreign key), seeded, one migration
- Pages: `/parcels`, `/parcels/{id}`, add, edit, delete, `/branches/{id}`
- Identity with Admin and Staff roles: Staff add and edit, only the Admin deletes
- A JSON API at `/api/parcels` (GET, POST, PUT, DELETE) with a DTO. **Not locked yet.**

## My work
- **Completed parts:** Parts A, B, C, D, E, F, and G.
  - **Part A:** Configured JWT authentication in `appsettings.json` and `Program.cs`, added `POST /api/auth/login` in `AuthController`, and locked `POST`, `PUT`, and `DELETE` on `/api/parcels` with JWT role/policy authorization while leaving `GET` endpoints anonymous and preserving cookie login for web pages.
  - **Part B:** Created `RequestLoggingMiddleware` to attach an 8-character `X-Request-Id` response header before invoking the pipeline and log method, path, status code, and elapsed milliseconds after `UseStaticFiles` and before `UseRouting`/`UseAuthentication`.
  - **Part C:** Added the `[RequireClient]` `ActionFilterAttribute` on API write actions to require a non-empty `X-Client` header (returning `400` if missing) and log before and after execution.
  - **Part D:** Created `IDeliveryFeeCalculator` and `DeliveryFeeCalculator`, registered it with DI, injected it into `ParcelsController` and `ParcelsApiController`, and displayed the fee on `/parcels/{id}` and in `ParcelDto`.
  - **Part E:** Implemented `[TrackingCode]` inheriting `ValidationAttribute` (`^PT-\d{6}$`, allowing empty values for `[Required]`) and applied it to `Parcel` and `ParcelInput`.
  - **Part F & G:** Updated `ParcelTrack.http` with all 10 sequential test requests and verified clean startup on a fresh database.
- **Part D Service Lifetime:** I chose **Singleton** because `DeliveryFeeCalculator` is completely stateless—it performs a pure calculation using only the arguments passed into its method and holds no per-request or database state, so one shared instance can safely serve every request without unnecessary allocations.
- **Known issues:** None; all features and all 10 `.http` requests work as specified.
