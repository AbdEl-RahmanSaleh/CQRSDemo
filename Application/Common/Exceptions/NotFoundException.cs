using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Exceptions;

public class NotFoundException(string entity, Guid id) : Exception($"{entity} with id {id} was not found.");

