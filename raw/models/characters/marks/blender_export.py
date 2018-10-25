
""" 
Exports character, clothing and held items to .fbx files with accompanying texture images. 
Export's directory hierarchy is determined by the dotted names of groups that objects are 
added to. Can be run from the command line:

    $ blender --background nova-kid.blend --python blender_export.py
    
"""

import os
import os.path
import shutil
import bpy
from mathutils import Quaternion, Vector

ARMATURE_NAME = 'Armature'

# ensure armature is in 'pose' mode
bpy.data.armatures[ARMATURE_NAME].pose_position = 'POSE'

# set armature to rest position, so it becomes default position in export
for pb in bpy.data.objects[ARMATURE_NAME].pose.bones:
    pb.rotation_quaternion = Quaternion((0,0,0),0)
    pb.scale = Vector((1,1,1))
    pb.location = Vector((0,0,0))

# export items in groups
blend_path = os.path.dirname(bpy.data.filepath)
export_path = os.path.join(blend_path, 'export')
for group in bpy.data.groups:
    # use directory structure from dotted group name
    group_path = os.path.join(export_path, *group.name.split('.'))
    # iterate over objects
    for obj in group.objects:
        obj_path = os.path.join(group_path, obj.name)
        os.makedirs(obj_path, exist_ok=True)
        selected = []
        # make sure the armature is exported for character items
        if group.name.startswith('character'):
            selected.append(bpy.data.objects['Armature'])
        selected.append(obj)
        # write .fbx file        
        bpy.ops.export_scene.fbx(
                { 'mode': 'OBJECT', 'selected_objects': selected },
                use_selection=True,
                version='ASCII6100',
                use_mesh_modifiers=True,
                filepath=os.path.join(obj_path, obj.name+'.fbx'))
        # iterate over texture images in object materials
        for matslot in obj.material_slots:
            if matslot is None:
                continue
            for texslot in matslot.material.texture_slots:
                if texslot is None:
                    continue
                if not isinstance(texslot.texture, bpy.types.ImageTexture): 
                    continue
                # copy texture image to exported directory hierarchy
                filename = texslot.texture.image.filepath.replace('//','')
                shutil.copy(os.path.join(blend_path, filename), 
                            os.path.join(obj_path, filename))
                                                  
