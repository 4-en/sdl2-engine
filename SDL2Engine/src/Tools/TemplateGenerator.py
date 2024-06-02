# simplify the template generation process

#
# SceneTemplates are used to load GameObjects from a file.
# The GameObjects are created by using named Prototype objects.
# Additionally, the GameObjects can have custom attributes set,
# for example, the position of the GameObject.
# 
# The files have the following format:
#    A list of top level XML elements, each representing a GameObject:
#       <Name />
#       or
#       <Name attribute1="value1" attribute2="value2" ... />
#       or
#       <Name attribute1="value1" attribute2="value2" ...>child1 child2 ...</Name>
#       
# Attributes can either be special defined attributes, like these:
#    count: the number of times the GameObject should be created
#    x: the x position of the GameObject
#    y: the y position of the GameObject
#    z: the z position of the GameObject
#    xr: the x random position offset of the GameObject (randDouble * xr)
#    yr: the y random position offset of the GameObject (randDouble * yr)
#    zr: the z random position offset of the GameObject (randDouble * zr)
# 
# or they can be attributes of the GameObject's components in the format:
#    ComponentClassName.attributeName[.subAttributeName]="value"
#

def CreateTemplate(name: str, **kwargs) -> str:
    return f"<{name} {' '.join([f'{key}="{value}"' for key, value in kwargs.items()])} />"

def DictToTemplate(dictionary: dict) -> str:
    name = dictionary.pop('name')
    return CreateTemplate(name, **dictionary)

def CreateTemplateFile(templates: list, filename: str):
    with open(filename, 'w') as file:
        for template in templates:
            file.write(DictToTemplate(template))
            file.write('\n')