﻿/* Copyright 2010-2013 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDB.Driver.GeoJsonObjectModel.Serializers
{
    public class GeoJsonLineStringCoordinatesSerializer<TCoordinates> : BsonBaseSerializer where TCoordinates : GeoJsonCoordinates
    {
        // private fields
        private readonly IBsonSerializer _coordinatesSerializer = BsonSerializer.LookupSerializer(typeof(TCoordinates));

        // public methods
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (bsonReader.GetCurrentBsonType() == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else
            {
                var positions = new List<TCoordinates>();

                bsonReader.ReadStartArray();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var position = (TCoordinates)_coordinatesSerializer.Deserialize(bsonReader, typeof(TCoordinates), null);
                    positions.Add(position);
                }
                bsonReader.ReadEndArray();

                return new GeoJsonLineStringCoordinates<TCoordinates>(positions);
            }
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var lineStringCoordinates = (GeoJsonLineStringCoordinates<TCoordinates>)value;

                bsonWriter.WriteStartArray();
                foreach (var position in lineStringCoordinates.Positions)
                {
                    _coordinatesSerializer.Serialize(bsonWriter, typeof(TCoordinates), position, null);
                }
                bsonWriter.WriteEndArray();
            }
        }
    }
}
