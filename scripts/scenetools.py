#!/usr/bin/env python3

import yaml
import io
import os
import re
import uuid
import pprint
import shutil


def load_scene(file_path):
    # load entire file as string, pre-processing as described here: 
    # http://stackoverflow.com/questions/21473076/pyyaml-and-unusual-tags
    string = ""
    with open(file_path,'r') as f:
        for line in f:
            if line.startswith('--- !u!'):
                docid = line.split()[2][1:]
                string += '--- &{0}\naliasID: {0}\n'.format(docid)
            else:
                string += line  
    
    # parse as yaml
    return list(yaml.load_all(io.StringIO(string)))


def scene_to_graph(data):
    graph = {}
    for doc in data:
        graph[doc['aliasID']] = {'doc': doc, 'refs': [], 'backrefs': [] }
    for node in graph.values():
        for ref in set(_collect_references(node['doc'], graph.keys())):
            node['refs'].append(graph[ref])
            graph[ref]['backrefs'].append(node)
    return graph


def find_graph_ghosts(data):
    result = { 'lone-prefabs': [], 'hidden-subgraphs': [] }
    visited_hiddens = set()
    for node in data.values():
        doc = node['doc']
        mainkey = _object_main_key(doc)
        # lone prefabs
        if mainkey == 'Prefab' and len(node['refs']) == 0 and len(node['backrefs']) == 0:
            result['lone-prefabs'].append(node)
        # hidden objects
        elif _object_is_hidden(doc, mainkey) and doc['aliasID'] not in visited_hiddens:
            hsub = { 'hidden': [node], 'non-hidden': [] }
            visited_hiddens.add(doc['aliasID'])
            connected_ids = _gather_connected(node, set())
            for connid in connected_ids:
                visited_hiddens.add(connid)
                connnode = data[connid]
                hsub['hidden' if _object_is_hidden(connnode['doc']) else 'non-hidden'].append(connnode)
            result['hidden-subgraphs'].append(hsub)
    return result


def graph_to_dot(data):
    s = ''
    s += 'digraph {\n'
    s += '    graph [pad="2", ranksep="2", nodesep="2"]\n'
    for node in data.values():
        doc = node['doc']
        mainkey = _object_main_key(doc)
        hidden = _object_is_hidden(doc, mainkey)
        s += '    "{0}" [label="{1} ({2})\\n{3}" color="{4}" fontcolor="{5}" style="{6}"]\n'.format(
                                    doc['aliasID'], doc[mainkey].get('m_Name',''), mainkey, doc['aliasID'],
                                    {'Transform': 'blue',
                                     'RectTransform': 'blue',
                                     'GameObject': 'darkorange',
                                     'Prefab': 'darkviolet',
                                     'MonoBehaviour': 'darkgreen'}.get(mainkey,'dimgrey'),
                                    'black' if hidden else 'white',
                                    'bold' if hidden else 'filled')
        for r in node['refs']:
            s += '    "{}" -> "{}"\n'.format(doc['aliasID'], r['doc']['aliasID'])
        s += '\n'
    s += '}\n'
    return s
    
    
def delete_objects(file_path, new_file_path, obj_ids):    
    obj_ids = set(obj_ids)
    copy_obj = True
    with open(file_path,'r') as fin, open(new_file_path,'w') as fout:
        for line in fin:
            match = re.match(r'^--- !u!\d+ &(\d+).*$', line)
            if match:
                matchid = int(match.group(1))
                if matchid in obj_ids:
                    obj_ids.remove(matchid)
                    copy_obj = False
                else:
                    copy_obj = True
            if copy_obj:
                fout.write(line)            


def _object_main_key(obj):
    return next(iter(set(obj.keys())-{'aliasID'}))


def _object_is_hidden(obj, mainkey=None):
    if mainkey is None:
        mainkey = _object_main_key(obj)
    return obj[mainkey].get('m_ObjectHideFlags',0) != 0


def _collect_references(obj, knownids):
    if isinstance(obj,dict):
        return ([obj['fileID']] if 'fileID' in obj and obj['fileID'] in knownids else []
                + sum([_collect_references(obj[k], knownids) for k in obj],[]))
    elif isinstance(obj, list):
        return sum([_collect_references(i, knownids) for i in obj],[])
    else:
        return []
        
        
def _gather_connected(node, visited):
    result = set()
    visited.add(node['doc']['aliasID'])
    for conn in node['refs']+node['backrefs']:
        nid = conn['doc']['aliasID']
        if nid in visited:
            continue
        result.add(nid)
        result.update(_gather_connected(conn, visited))
    return result
    
    
def _confirm_prompt(message, default):
    while True:
        resp = input('{} [{}/{}]: '.format(message, 'Y' if default else 'y', 'N' if not default else 'n'))
        if resp.strip().lower() == 'y':
            return True
        elif resp.strip().lower() == 'n':
            return False
        elif resp.strip() == '':
            return default


if __name__ == '__main__':
    
    import argparse    
    
    def obj_summary(obj):
        mainkey = _object_main_key(obj)
        return '{} ({}) {}'.format(obj[mainkey].get('m_Name','<NoName>'), mainkey, obj['aliasID'])
    
    def do_graph(args):
        print(graph_to_dot(scene_to_graph(load_scene(args.scenefile))))
    
    def do_ghosts(args):
        found = find_graph_ghosts(scene_to_graph(load_scene(args.scenefile)))
        to_del = set()
        for lpfab in found['lone-prefabs']:
            obj_id = lpfab['doc']['aliasID']
            if args.noprompt or _confirm_prompt('Delete lone prefab {} ?'.format(obj_id), True):
                to_del.add(obj_id)
        for hsub in found['hidden-subgraphs']:
            obj_ids = set()     
            if not args.noprompt:
                print('Hidden:')
            for h in hsub['hidden']:
                obj_ids.add(h['doc']['aliasID'])
                if not args.noprompt:
                    print(' '*4 + obj_summary(h['doc']))
            if not args.noprompt:
                print('Non-Hidden:')
            for n in hsub['non-hidden']:
                obj_ids.add(n['doc']['aliasID'])
                if not args.noprompt:
                    print(' '*4 + obj_summary(n['doc']))
            if args.noprompt or _confirm_prompt('Delete this hidden subgraph?', True):
                to_del.update(obj_ids)
        if len(to_del) > 0:
            tempname = '{}.temp'.format(args.scenefile)
            delete_objects(args.scenefile, tempname, to_del)
            if not args.nobackup:
                backname = '{}.backup'.format(args.scenefile)
                shutil.move(args.scenefile, backname)
            else:
                os.remove(args.scenefile)
            shutil.move(tempname, args.scenefile)
    
    ap = argparse.ArgumentParser()
    subs = ap.add_subparsers()            
        
    graph = subs.add_parser('graph', help='Outputs a dot graph representing the objects in the given scene file')
    graph.add_argument('scenefile')
    graph.set_defaults(func=do_graph)    
            
    ghosts = subs.add_parser('ghosts', help='Removes ghost objects from the given scene file')    
    ghosts.add_argument('scenefile')
    ghosts.add_argument('-b','--nobackup',action='store_true',help="Don't keep a backup of the original file")
    ghosts.add_argument('-y','--noprompt',action='store_true',help="Don't ask before deleting")
    ghosts.set_defaults(func=do_ghosts)
            
    args = ap.parse_args()
    args.func(args)    
