# My API Practice Repository

This repository contains multiple APIs for practicing and improving backend development skills using .NET.

## APIs Included
1. **CityInfo.API** - A CRUD API for managing cities and points of interest, with authentication.
2. **CourseLibrary.API** - A REST API with advanced features like HATEOAS, data shaping, paging, and sorting.
3. **GloboTicket.TicketManagement** - A clean architecture API using the Mediator pattern (MediatR), CQRS, and pagination.

---

## 1️⃣ CityInfo.API
A simple CRUD API with three controllers:

### **Endpoints**
#### **City Controller (`/api/cities`)**
| Method | Endpoint              | Description                    |
|--------|----------------------|--------------------------------|
| `GET`  | `/api/cities`         | Get all cities                 |
| `GET`  | `/api/cities/{cityId}`| Get a specific city            |
| `POST` | `/api/cities`         | Add a new city                 |
| `PUT`  | `/api/cities/{cityId}`| Update a city                  |
| `PATCH`| `/api/cities/{cityId}`| Partially update a city        |
| `DELETE`| `/api/cities/{cityId}`| Delete a city                  |

#### **Point of Interest Controller (`/api/cities/{cityId}/pointsOfInterest`)**
| Method | Endpoint                                      | Description                        |
|--------|---------------------------------------------|------------------------------------|
| `GET`  | `/api/cities/{cityId}/pointsOfInterest`     | Get all points of interest for a city |
| `GET`  | `/api/cities/{cityId}/pointsOfInterest/{pointOfInterestId}`| Get a specific point of interest   |
| `POST` | `/api/cities/{cityId}/pointsOfInterest`     | Add a new point of interest        |
| `PUT`  | `/api/cities/{cityId}/pointsOfInterest/{pointOfInterestId}`| Update a point of interest         |
| `PATCH`| `/api/cities/{cityId}/pointsOfInterest/{pointOfInterestId}`| Partially update a point of interest |
| `DELETE`| `/api/cities/{cityId}/pointsOfInterest/{pointOfInterestId}`| Delete a point of interest        |

#### **Authorization Controller (`/api/authentication/authenticate`)**
| Method | Endpoint                              | Description         |
|--------|--------------------------------------|---------------------|
| `POST` | `/api/authentication/authenticate`  | Authenticate a user |

---

## 2️⃣ CourseLibrary.API
A REST API with advanced features such as HATEOAS, data shaping, paging, and sorting.

### **Endpoints**
#### **Root Controller (`/api`)**
| Method | Endpoint  | Description                    |
|--------|---------|--------------------------------|
| `GET`  | `/api`  | Provides links to navigate other controllers |

#### **Authors Controller (`/api/authors`)**
| Method | Endpoint                         | Description                                  |
|--------|---------------------------------|----------------------------------------------|
| `GET`  | `/api/authors`                   | Get all authors (one with links, one without) |
| `GET`  | `/api/authors/{authorId}`        | Get a specific author (friendly & full, with/without links) |
| `POST` | `/api/authors`                    | Add a new author (with or without date of death) |
| `PUT`  | `/api/authors/{authorId}`        | Update an author                            |
| `PATCH`| `/api/authors/{authorId}`        | Partially update an author                  |
| `DELETE`| `/api/authors/{authorId}`       | Remove an author                            |
| `OPTIONS`| `/api/authors`                 | Retrieve allowed HTTP methods               |

#### **Author Collections Controller (`/api/authorCollections`)**
| Method | Endpoint                          | Description                                 |
|--------|----------------------------------|---------------------------------------------|
| `GET`  | `/api/authorCollections/({authorIds})`   | Retrieve a collection of authors by IDs    |
| `POST` | `/api/authorCollections`         | Create a collection of authors in one request |
| `DELETE`| `/api/authorCollections/({authorIds})`  | Remove a collection of authors by IDs      |

#### **Courses Controller (`/api/courses`)**
| Method | Endpoint              | Description                    |
|--------|----------------------|--------------------------------|
| `GET`  | `/api/authors/{authorId}/courses`        | Get all courses (supports data shaping, paging, sorting) |
| `GET`  | `/api/authors/{authorId}/courses/{courseId}`| Get a specific course (supports data shaping) |
| `POST` | `/api/authors/{authorId}/courses`        | Add a new course              |
| `PUT`  | `/api/authors/{authorId}/courses/{courseId}`| Update a course             |
| `PATCH`| `/api/authors/{authorId}/courses/{courseId}`| Partially update a course   |
| `DELETE`| `/api/authors/{authorId}/courses/{courseId}`| Delete a course             |
| `OPTIONS`| `/api/authors/{authorId}/courses`      | Retrieve allowed HTTP methods |

---

## 3️⃣ GloboTicket.TicketManagement
A practice API for implementing clean architecture, CQRS, and the Mediator pattern (MediatR).

### **Endpoints**
#### **Orders Controller (`/api/orders`)**
| Method | Endpoint        | Description                 |
|--------|--------------|-----------------------------|
| `GET`  | `/api/orders` | Get all orders (supports paging) |

#### **Events Controller (`/api/events`)**
| Method | Endpoint              | Description                    |
|--------|----------------------|--------------------------------|
| `GET`  | `/api/events`         | Get all events                 |
| `GET`  | `/api/events/{eventId}`| Get a specific event           |
| `GET`  | `/api/events/export`  | Export events to CSV           |
| `POST` | `/api/events`         | Add a new event                |
| `PUT`  | `/api/events`| Update an event               |
| `DELETE`| `/api/events/{id}`| Delete an event               |

#### **Categories Controller (`/api/categories`)**
| Method | Endpoint               | Description                    |
|--------|----------------------|--------------------------------|
| `GET`  | `/api/categories`      | Get all categories             |
| `GET`  | `/api/categories/{id}`| Get a specific category  |
| `POST` | `/api/categories`      | Add a new category             |

---

## Authentication
To log in, send a `POST` request to `/api/authentication/authenticate` with valid credentials.

Example request (JSON):
```json
{
  "username": "username",
  "password": "password"
}
```
The API will return a JWT token upon successful authentication.

